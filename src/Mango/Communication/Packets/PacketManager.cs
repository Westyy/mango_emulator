using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Mango.Communication.Packets.Incoming.Advertisement;
using Mango.Communication.Packets.Incoming.Catalog;
using Mango.Communication.Packets.Incoming.Handshake;
using Mango.Communication.Packets.Incoming.Inventory.Purse;
using Mango.Communication.Packets.Incoming.Messenger;
using Mango.Communication.Packets.Incoming.Navigator;
using Mango.Communication.Packets.Incoming.Room.Chat;
using Mango.Communication.Packets.Incoming.Room.Connection;
using Mango.Communication.Packets.Incoming.Room.Engine;
using Mango.Communication.Packets.Incoming.Room.Settings;
using Mango.Communication.Packets.Incoming.Tracking;
using Mango.Communication.Packets.Incoming.Users;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Incoming;
using Mango.Communication.Packets.Incoming.Inventory.Furni;
using Mango.Communication.Packets.Incoming.Inventory.Badges;
using Mango.Communication.Packets.Incoming.Inventory.Achievements;
using Mango.Communication.Packets.Incoming.Quest;
using Mango.Communication.Packets.Incoming.Room.Avatar;
using Mango.Communication.Packets.Incoming.Sound;
using System.Threading;
using System;
using System.Collections.Concurrent;
using Mango.Communication.Packets.Incoming.Room.Action;
using Mango.Communication.Packets.Incoming.Register;
using Mango.Communication.Packets.Incoming.Avatar;
using Mango.Communication.Packets.Incoming.Room.Furniture;
using Mango.Communication.Packets.Incoming.Moderator;
using Mango.Communication.Packets.Incoming.Inventory.AvatarEffects;
using Mango.Communication.Packets.Incoming.Games;

namespace Mango.Communication.Packets
{
    sealed class PacketManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Communication.Packets.PacketManager");

        /// <summary>
        /// The collection that stores all the packets to a packet id as the key.
        /// </summary>
        private readonly Dictionary<int, IPacketEvent> _packets;

        /// <summary>
        /// The task factory which is used for running Asynchronous tasks, in this case we use it to execute packets.
        /// </summary>
        private readonly TaskFactory _packetHandler;

        /// <summary>
        /// Currently running tasks to keep track of what the current load is
        /// </summary>
        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        /// <summary>
        /// Testing the Task code
        /// </summary>
        private readonly bool IgnoreTasks = false;

        /// <summary>
        /// The maximum time a task can run for before it is considered dead
        /// (can be used for debugging any locking issues with certain areas of code)
        /// </summary>
        private readonly int MaximumRunTimeInSec = 300; // 5 minutes

        /// <summary>
        /// Should the handler throw errors or log and continue.
        /// </summary>
        private readonly bool ThrowUserErrors = false;

        /// <summary>
        /// Initializes a new instance of the PacketManager.
        /// </summary>
        public PacketManager()
        {
            this._packets = new Dictionary<int, IPacketEvent>();
            this._packetHandler = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            this._runningTasks = new ConcurrentDictionary<int, Task>();
        }

        /// <summary>
        /// Handles the packet provided
        /// </summary>
        /// <param name="Session">The session which invokes this packet.</param>
        /// <param name="Packet">The client packet received.</param>
        public void ExecutePacket(Session Session, ClientPacket Packet)
        {
            IPacketEvent Pak = null;

            if (!_packets.TryGetValue(Packet.Id, out Pak))
            {
                log.Debug("Unhandled packet " + Packet.Id);
                return;
            }

            log.Debug("<Session " + Session.Id + "> Executing packet: " + Packet.Id);

            if (!IgnoreTasks)
            {
                DateTime Start = DateTime.Now;

                var CancelSource = new CancellationTokenSource();
                var Token = CancelSource.Token;

                Task t = _packetHandler.StartNew(() =>
                {
                    Pak.parse(Session, Packet);
                    Token.ThrowIfCancellationRequested();
                }, Token);

                this._runningTasks.TryAdd(t.Id, t);

                try
                {
                    if (!t.Wait(MaximumRunTimeInSec * 1000, Token))
                    {
                        CancelSource.Cancel();
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.Flatten().InnerExceptions)
                    {
                        if (ThrowUserErrors)
                        {
                            throw e;
                        }
                        else
                        {
                            log.Fatal("Unhandled Error: " + e.Message + " - " + e.StackTrace);
                            Session.Disconnect();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Session.Disconnect();
                }
                finally
                {
                    Task RemovedTask = null;
                    this._runningTasks.TryRemove(t.Id, out RemovedTask);

                    CancelSource.Dispose();

                    log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
                }
            }
            else
            {
                Pak.parse(Session, Packet);
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in this._runningTasks.Values)
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            this._packets.Clear();
        }

        public void RegisterGames()
        {
            this._packets.Add(ClientPacketHeader.InitGameCenterEvent, new InitGameCenterEvent());
            this._packets.Add(ClientPacketHeader.GetGameAchievementsEvent, new GetGameAchievementsEvent());
        }

        public void RegisterCatalog()
        {
            this._packets.Add(ClientPacketHeader.GetClubOffersMessageEvent, new GetClubOffersEvent());
            this._packets.Add(ClientPacketHeader.GetCatalogIndexEvent, new GetCatalogIndexEvent());
            this._packets.Add(ClientPacketHeader.GetCatalogPageEvent, new GetCatalogPageEvent());
            this._packets.Add(ClientPacketHeader.PurchaseFromCatalogEvent, new PurchaseFromCatalogEvent());
        }

        public void RegisterHandshake()
        {
            this._packets.Add(ClientPacketHeader.GetClientVersionEvent, new GetClientVersionEvent());
            this._packets.Add(ClientPacketHeader.DisconnectMessageEvent, new DisconnectEvent());
            this._packets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            this._packets.Add(ClientPacketHeader.GetSessionParametersMessageEvent, new GetSessionParametersEvent());
            this._packets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            this._packets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            this._packets.Add(ClientPacketHeader.PongMessageEvent, new PongEvent());
            this._packets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SSOTicketEvent());
            this._packets.Add(ClientPacketHeader.TryLoginMessageEvent, new TryLoginEvent());
            this._packets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIDEvent());
            this._packets.Add(ClientPacketHeader.VersionCheckMessageEvent, new VersionCheckEvent());
        }

        public void RegisterInventoryAchievements()
        {
            this._packets.Add(ClientPacketHeader.GetAchievementsEvent, new GetAchievementsEvent());
        }

        public void RegisterInventoryAvatarEffects()
        {
            this._packets.Add(ClientPacketHeader.AvatarEffectActivatedEvent, new AvatarEffectActivatedEvent());
            this._packets.Add(ClientPacketHeader.AvatarEffectSelectedEvent, new AvatarEffectSelectedEvent());
        }

        public void RegisterInventoryBadges()
        {
            this._packets.Add(ClientPacketHeader.GetBadgesEvent, new GetBadgesEvent());
            this._packets.Add(ClientPacketHeader.SetActivatedBadgesEvent, new SetActivatedBadgesEvent());
        }

        public void RegisterInventoryFurni()
        {
            this._packets.Add(ClientPacketHeader.RequestFurniInventoryEvent, new RequestFurniInventoryEvent());
        }

        public void RegisterInventoryPurse()
        {
            this._packets.Add(ClientPacketHeader.GetCreditsInfoEvent, new GetCreditsInfoEvent());
        }

        public void RegisterMessenger()
        {
            this._packets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            this._packets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
            this._packets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            this._packets.Add(ClientPacketHeader.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            this._packets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            this._packets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            this._packets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            this._packets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            this._packets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            this._packets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            this._packets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            this._packets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
        }

        public void RegisterModerator()
        {
            this._packets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            this._packets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            this._packets.Add(ClientPacketHeader.ModMessageMessageEvent, new ModMessageMessageEvent());
            this._packets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionMessageEvent());
            this._packets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            this._packets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            this._packets.Add(ClientPacketHeader.GetRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            this._packets.Add(ClientPacketHeader.GetUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            this._packets.Add(ClientPacketHeader.GetCfhChatlogMessageEvent, new GetModeratorIssueChatlogEvent());
            this._packets.Add(ClientPacketHeader.GetRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            this._packets.Add(ClientPacketHeader.CallForHelpMessageEvent, new SubmitNewTicketEvent());
        }

        public void RegisterNavigator()
        {
            this._packets.Add(ClientPacketHeader.GetOfficialRoomsMessageEvent, new GetOfficialRoomsEvent());
            this._packets.Add(ClientPacketHeader.GetPopularRoomTagsMessageEvent, new GetPopularRoomTagsEvent());
            this._packets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            this._packets.Add(ClientPacketHeader.LatestEventsSearchMessageEvent, new LatestEventsSearchEvent());
            this._packets.Add(ClientPacketHeader.MyRoomsSearchMessageEvent, new MyRoomsSearchMessageEvent());
            this._packets.Add(ClientPacketHeader.PopularRoomsSearchMessageEvent, new PopularRoomsSearchEvent());
            this._packets.Add(ClientPacketHeader.RoomsWithHighestScoreSearchMessageEvent, new RoomsWithHighestScoreSearchEvent());
            this._packets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            this._packets.Add(ClientPacketHeader.CanCreateEventMessageEvent, new CanCreateEventEvent());
            /*this.Packets.Add(ClientPacketHeader.CreateEventMessageEvent, new CreateEventEvent());
            this.Packets.Add(ClientPacketHeader.EditEventMessageEvent, new EditEventEvent());
            this.Packets.Add(ClientPacketHeader.CancelEventMessageEvent, new CancelEventEvent());*/
            this._packets.Add(ClientPacketHeader.GetPublicSpaceCastLibsMessageEvent, new GetPublicSpaceCastLibsEvent());
            this._packets.Add(ClientPacketHeader.RoomTextSearchMessageEvent, new RoomTextSearchEvent());
            this._packets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            this._packets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            this._packets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            this._packets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            this._packets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            this._packets.Add(ClientPacketHeader.MyFavouriteRoomsSearchMessageEvent, new GetFavouriteRoomsEvent());
            this._packets.Add(ClientPacketHeader.PromotedRoomsEvent, new PromotedRoomsEvent());
        }

        public void RegisterAdvertisement()
        {
            this._packets.Add(ClientPacketHeader.GetRoomAdMessageEvent, new GetRoomAdEvent());
        }

        public void RegisterAvatar()
        {
            this._packets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            this._packets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        public void RegisterQuest()
        {
            this._packets.Add(ClientPacketHeader.GetQuestsMessageEvent, new GetQuestsEvent());
        }

        public void RegisterRegister()
        {
            this._packets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
        }

        public void RegisterRoomAvatar()
        {
            this._packets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            this._packets.Add(ClientPacketHeader.SignMessageEvent, new SignEvent());
            this._packets.Add(ClientPacketHeader.WaveMessageEvent, new WaveEvent());
            this._packets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            this._packets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            this._packets.Add(ClientPacketHeader.SitAvatarEvent, new SitEvent());
        }

        public void RegisterRoomChat()
        {
            this._packets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            this._packets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            this._packets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            this._packets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
            this._packets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
        }

        public void RegisterRoomAction()
        {
            this._packets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            this._packets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            this._packets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            this._packets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            this._packets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            this._packets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsComposer());
        }

        public void RegisterRoom()
        {
            this._packets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
            this._packets.Add(ClientPacketHeader.QuitMessageEvent, new QuitEvent());
            this._packets.Add(ClientPacketHeader.OpenConnectionMessageEvent, new OpenConnectionEvent());
            this._packets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            this._packets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            this._packets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            this._packets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            this._packets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            this._packets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            this._packets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            this._packets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            this._packets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            this._packets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            this._packets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            this._packets.Add(ClientPacketHeader.ApplyDecorationEvent, new ApplyDecorationEvent());

            this._packets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            this._packets.Add(ClientPacketHeader.UseWallItemMessageEvent, new UseWallItemEvent());
        }

        public void RegisterRoomFurniture()
        {
            this._packets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            this._packets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new ThrowDiceEvent());
        }

        public void RegisterSound()
        {
            this._packets.Add(ClientPacketHeader.GetSoundSettingsEvent, new GetSoundSettingsEvent());
            this._packets.Add(ClientPacketHeader.SetSoundSettingsEvent, new SetSoundSettingsEvent());
        }

        public void RegisterTracking()
        {
            this._packets.Add(ClientPacketHeader.EventLogMessageEvent, new EventLogEvent());
        }

        public void RegisterUsers()
        {
            this._packets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            this._packets.Add(ClientPacketHeader.GetMOTDMessageEvent, new GetMOTDEvent());
            this._packets.Add(ClientPacketHeader.GetUserNotificationsMessageEvent, new GetUserNotificationsEvent());
            this._packets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            this._packets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            this._packets.Add(ClientPacketHeader.GetProfileInformationEvent, new GetProfileInformationEvent());
        }
    }
}
