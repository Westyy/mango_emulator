using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using log4net;
using Mango.Communication.Packets.Incoming;
using Mango.Players;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing;
using Mango.Communication.Packets.Outgoing.Handshake;
using Mango.Communication.Packets.Outgoing.Availability;
using Mango.Communication.Packets.Outgoing.Notifications;
using Mango.Communication.Packets.Outgoing.Inventory.Purse;
using System.Collections.Concurrent;
using System.Threading;
using Mango.Communication.Packets.Outgoing.Users;
using Mango.Subscriptions;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Mango.Communication.Encryption;
using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Communication.Packets.Outgoing.Room.Session;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Communication.Sessions
{
    sealed class Session
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Communication.Sessions.Session");

        /// <summary>
        /// Unique ID which indentifies this Session. (It may only be used for debugging purposes)
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The Socket used for the backend of the communication.
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// The manager that was used to create this session.
        /// </summary>
        public ServerSocket Manager { get; private set; }

        public SocketAsyncEventArgs SendEventArgs { get; set; }

        private readonly ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();

        private int WriterCount = 0;

        //private EventWaitHandle DisconnectWait = new EventWaitHandle(false, EventResetMode.AutoReset);
        private AutoResetEvent DisconnectWait = new AutoResetEvent(true);

        public int DesignedHandler { get; set; }
        public RC4 RC4Client = null;
        public bool CryptoInitialized { get; set; }

        public string MachineId { get; set; }

        /// <summary>
        /// The game player linked to this active session.
        /// </summary>
        private Player Player = null;

        private SessionPacketHandler PacketHandler = null;

        /// <summary>
        /// Initializes a new instance of the Session class.
        /// </summary>
        /// <param name="manager">The manager which created this session.</param>
        public Session(int id, ServerSocket Manager, SocketAsyncEventArgs sendEventArgs)
        {
            this.Id = id;
            this.Manager = Manager;
            this.SendEventArgs = sendEventArgs;

            this.PacketHandler = new SessionPacketHandler();
        }

        /// <summary>
        /// Gets the IP Address of this connection session.
        /// </summary>
        public string IPAddress
        {
            get
            {
                return this.Socket.RemoteEndPoint.ToString().Split(':')[0];
            }
        }

        bool HalfDataRcv = false;
        byte[] HalfData = null;

        public void OnReceiveData(byte[] Data)
        {
            if (this.DisconnectedCalled) // stops any packets from processing
            {
                return;
            }

            this.CryptoInitialized = false;

            if (HalfDataRcv)
            {
                byte[] FullDataRcv = new byte[HalfData.Length + Data.Length];
                Buffer.BlockCopy(HalfData, 0, FullDataRcv, 0, HalfData.Length);
                Buffer.BlockCopy(Data, 0, FullDataRcv, HalfData.Length, Data.Length);

                HalfDataRcv = false; // mark done this round
                OnReceiveData(FullDataRcv); // repeat now we have the combined array
                return;
            }

            if (Data[0] == 60)
            {
                string Policy = "<?xml version=\"1.0\"?>\r\n"
                    + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                    + "<cross-domain-policy>\r\n"
                    + "<site-control permitted-cross-domain-policies=\"master-only\"/>\r\n"
                    + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                    + "</cross-domain-policy>";

                SendData(Policy);
                Disconnect();
            }
            else if (Data[0] != 67)
            {
                try // SO LAZYYYYYYYYY NEED FIX THIS OMG :(
                {
                    using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                    {
                        int MsgLen = HabboEncoding.DecodeInt32(this.CryptoInitialized ? this.RC4Client.Parse(Reader.ReadBytes(4)) : Reader.ReadBytes(4));

                        if ((Reader.BaseStream.Length - 4) < MsgLen) // was 3
                        {
                            HalfData = Data;
                            HalfDataRcv = true;
                            return;
                        }

                        byte[] Content = this.CryptoInitialized ? this.RC4Client.Parse(Reader.ReadBytes(MsgLen)) : Reader.ReadBytes(MsgLen);

                        using (BinaryReader SecondaryReader = new BinaryReader(new MemoryStream(Content)))
                        {
                            int MsgId = HabboEncoding.DecodeInt16(SecondaryReader.ReadBytes(2));

                            byte[] ReadableContent = new byte[(Content.Length - 2)];
                            Buffer.BlockCopy(Content, 2, ReadableContent, 0, (Content.Length - 2));

                            ClientPacket Packet = ClientPacketFactory.CreateNew();
                            Packet.Initialize(MsgId, ReadableContent);

                            OnPacketReady(Packet);
                        }

                        if ((Reader.BaseStream.Length - 4) > MsgLen)
                        {
                            byte[] MoreContent = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                            Buffer.BlockCopy(Data, (int)Reader.BaseStream.Position, MoreContent, 0, (int)(Reader.BaseStream.Length - Reader.BaseStream.Position));

                            OnReceiveData(MoreContent);
                        }
                    }
                }
                catch { Disconnect(); }
            }
        }

        /// <summary>
        /// This method is called for when a packet is ready to be read.
        /// </summary>
        /// <param name="packet"></param>
        private void OnPacketReady(ClientPacket Packet)
        {
            this.PacketHandler.ExecutePacket(this, Packet);
        }

        /// <summary>
        /// This method is called when the Session has disconnected.
        /// </summary>
        public void OnDisconnection()
        {
            if (this.Player != null)
            {
                this.Player.Cleanup();
                this.Player = null;
            }

            this.DesignedHandler = 1;
            this.RC4Client = null;
            this.CryptoInitialized = false;
            this.HalfData = null;
            this.HalfDataRcv = false;

            this.MachineId = string.Empty;

            this.PacketHandler.Reset();
            this.DisconnectedCalled = false; // reset this value!!!!

            this.DisconnectWait.Set();
        }

        private bool DisconnectedCalled = false;

        /// <summary>
        /// Forces this player to be disconnected
        /// </summary>
        public void Disconnect()
        {
            if (!this.DisconnectedCalled)
            {
                this.DisconnectedCalled = true;
                this.Socket.Shutdown(SocketShutdown.Both);

                this.DisconnectWait.WaitOne();
            }
        }

        public void SendPacket(ServerPacket packet)
        {
            this.SendData(packet.GetBytes());
        }

        public void SendData(string Data)
        {
            SendData(Encoding.UTF8.GetBytes(Data));
        }

        public void SendData(byte[] data)
        {
            QueueToSend(data);
        }

        public void QueueToSend(byte[] data)
        {
            this.SendQueue.Enqueue(data);
            PromptToSend();
        }

        public void PromptToSend()
        {
            if (Interlocked.CompareExchange(ref WriterCount, 1, 0) == 0)
            {
                StartSending();
            }
        }

        public void StartSending()
        {
            byte[] result = null;

            if (this.SendQueue.TryDequeue(out result))
            {
                SendDataToken token = (SendDataToken)SendEventArgs.UserToken;
                token.DataToSend = result;
                token.SendBytesRemainingCount = result.Length;

                this.Manager.BeginSend(SendEventArgs);
            }
            else
            {
                Interlocked.Exchange(ref WriterCount, 0);
            }
        }

        public void OnSendCompleted()
        {
            StartSending();
        }

        public void TryAuthenticate(string SSOTicket)
        {
            PlayerData Data = null;

            if (!SSOAuthenticator.TryAuthenticate(SSOTicket, IPAddress, out Data))
            {
                Disconnect();
                return;
            }

            Player CurrentPlayer = null;

            if (Mango.GetServer().GetPlayerManager().TryGet(Data.Id, out CurrentPlayer))
            {
                CurrentPlayer.GetSession().Disconnect();
            }

            Player Player = new Player(this, Data);
            SetPlayer(Player);

            if (!Player.InitializeComponents())
            {
                Disconnect();
                return;
            }

            // perform this last
            if (!Mango.GetServer().GetPlayerManager().TryAdd(Player))
            {
                Disconnect(); // player manager told us something went wrong.. remove this player
                return;
            }

            // tell client we are all ok
            SendPacket(new AuthenticationOKComposer());
            SendPacket(new AvailabilityStatusComposer());
            SendPacket(new NavigatorSettingsComposer(Player.HomeRoom));
            SendPacket(new UserRightsComposer(Player.GetPermissions().HasRight("club_regular"), Player.GetPermissions().HasRight("club_vip"), Player.GetPermissions().HasRight("super_admin")));
            SendPacket(new AvatarEffectsComposer(Player.Effects().GetAllEffects));
            SendPacket(new GetMinimailMessageCountComposer());

            Subscription ActiveSubscription = null;

            if (Player.GetSubscriptions().TryGetSubscription("habbo_club", out ActiveSubscription))
            {
                SendPacket(new ScrSendUserInfoComposer(ActiveSubscription, false));
            }
            else
            {
                SendPacket(new ScrSendUserInfoComposer(null, false));
            }

            Player.GetMessenger().SetUpdateNeeded(true);

            if (Player.GetPermissions().HasRight("mod_tool"))
            {
                SendPacket(new ModeratorInitMessageComposer(Player.GetPermissions().HasRight("mod_tickets"),
                    Mango.GetServer().GetModerationManager().UserMessagePresets,
                    Mango.GetServer().GetModerationManager().RoomMessagePresets,
                    Mango.GetServer().GetModerationManager().UserActionPresets));

                // todo: tickets sent
            }

            // Room Favourites
            SendPacket(new FavouritesComposer(Player.Favourites().FavouriteRoomsId));

            Player.InitProcess(); // always run this last please ty

            if (!string.IsNullOrEmpty(MangoStaticGameSettings.LoginWelcomeMessage))
            {
                SendPacket(new HabboBroadcastComposer(MangoStaticGameSettings.LoginWelcomeMessage));
            }
        }

        /// <summary>
        /// Sets the player for this session.
        /// </summary>
        /// <param name="Player"></param>
        public void SetPlayer(Player Player)
        {
            if (this.Player != null)
            {
                throw new InvalidOperationException("Cannot set a player that has already been set."); // makes life easier when debugging...
            }

            this.Player = Player;
        }

        /// <summary>
        /// Retrieves the active player for this session.
        /// </summary>
        /// <returns></returns>
        public Player GetPlayer()
        {
            if (this.Player == null)
            {
                throw new NullReferenceException("Cannot access a player that has been disposed."); // makes life easier when debugging...
            }

            return this.Player;
        }
    }
}
