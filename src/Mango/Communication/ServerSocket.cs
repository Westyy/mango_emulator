using System;
using System.Net.Sockets;
using System.Threading;
using log4net;
using Mango.Communication.Sessions;

namespace Mango.Communication
{
    sealed class ServerSocket
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Communication.ServerSocket");

        /// <summary>
        /// The settings to use with this ServerSocket.
        /// </summary>
        private ServerSocketSettings Settings;

        /// <summary>
        /// The buffer manager for allocation a buffer block to a SocketAsyncEventArgs.
        /// </summary>
        private BufferManager BufferManager;

        /// <summary>
        /// The semaphore used for controlling the max connections to the server.
        /// </summary>
        private SemaphoreSlim MaxConnectionsEnforcer;

        /// <summary>
        /// The semaphore used for controlling the max socket accept operations at a time.
        /// </summary>
        private SemaphoreSlim MaxAcceptOpsEnforcer;

        /// <summary>
        /// The socket used for listening for incoming connections.
        /// </summary>
        private Socket ListenSocket;

        /// <summary>
        /// The pool of re-usable SocketAsyncEventArgs for accept operations.
        /// </summary>
        private SocketAsyncEventArgsPool PoolOfAcceptEventArgs;

        /// <summary>
        /// The pool of re-usable SocketAsyncEventArgs for receiving data.
        /// </summary>
        private SocketAsyncEventArgsPool PoolOfRecEventArgs;

        /// <summary>
        /// Initializes a new instance of the ServerSocket.
        /// </summary>
        /// <param name="settings">The settings to use with this ServerSocket.</param>
        public ServerSocket(ServerSocketSettings settings)
        {
            this.Settings = settings;

            this.BufferManager = new BufferManager((this.Settings.BufferSize * this.Settings.NumOfSaeaForRec) * 2, this.Settings.BufferSize);
            this.PoolOfAcceptEventArgs = new SocketAsyncEventArgsPool(this.Settings.MaxSimultaneousAcceptOps);
            this.PoolOfRecEventArgs = new SocketAsyncEventArgsPool(this.Settings.NumOfSaeaForRec);

            this.MaxConnectionsEnforcer = new SemaphoreSlim(this.Settings.MaxConnections, this.Settings.MaxConnections);
            this.MaxAcceptOpsEnforcer = new SemaphoreSlim(this.Settings.MaxSimultaneousAcceptOps, this.Settings.MaxSimultaneousAcceptOps);
        }

        public void Init()
        {
            this.BufferManager.InitBuffer();

            for (int i = 0; i < this.Settings.MaxSimultaneousAcceptOps; i++)
            {
                SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed +=
                    new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                this.PoolOfAcceptEventArgs.Push(acceptEventArg);
            }

            // receive objects
            for (int i = 0; i < this.Settings.NumOfSaeaForRec; i++)
            {
                SocketAsyncEventArgs recvEventArgObjectForPool = new SocketAsyncEventArgs();
                this.BufferManager.SetBuffer(recvEventArgObjectForPool);

                recvEventArgObjectForPool.Completed +=
                    new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                SocketAsyncEventArgs sendEventArgObject = new SocketAsyncEventArgs();
                this.BufferManager.SetBuffer(sendEventArgObject);

                sendEventArgObject.Completed +=
                    new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                recvEventArgObjectForPool.UserToken = new Session(i, this, sendEventArgObject);
                sendEventArgObject.UserToken = new SendDataToken((Session)recvEventArgObjectForPool.UserToken);

                this.PoolOfRecEventArgs.Push(recvEventArgObjectForPool);
            }
        }

        public void StartListen()
        {
            this.ListenSocket = new Socket(this.Settings.Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.ListenSocket.Bind(this.Settings.Endpoint);
            this.ListenSocket.Listen(this.Settings.Backlog);

            StartAccept();
        }

        private void StartAccept()
        {
            SocketAsyncEventArgs acceptEventArgs;

            this.MaxAcceptOpsEnforcer.Wait();

            if (this.PoolOfAcceptEventArgs.TryPop(out acceptEventArgs))
            {
                this.MaxConnectionsEnforcer.Wait();
                bool willRaiseEvent = this.ListenSocket.AcceptAsync(acceptEventArgs);

                if (!willRaiseEvent)
                {
                    ProcessAccept(acceptEventArgs);
                }
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            StartAccept();

            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                HandleBadAccept(acceptEventArgs);
                this.MaxAcceptOpsEnforcer.Release();
                return;
            }

            SocketAsyncEventArgs recEventArgs;

            if (this.PoolOfRecEventArgs.TryPop(out recEventArgs))
            {
                ((Session)recEventArgs.UserToken).Socket = acceptEventArgs.AcceptSocket;
                ((Session)recEventArgs.UserToken).SendEventArgs.AcceptSocket = ((Session)recEventArgs.UserToken).Socket;

                acceptEventArgs.AcceptSocket = null;
                this.PoolOfAcceptEventArgs.Push(acceptEventArgs);
                this.MaxAcceptOpsEnforcer.Release();

                log.Debug("<Session " + ((Session)recEventArgs.UserToken).Id + "> is now in use.");

                StartReceive(recEventArgs);
            }
            else
            {
                HandleBadAccept(acceptEventArgs);
                log.Fatal("Cannot handle this session, there are no more receive objects available for us.");
            }
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;

                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;

                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;

                default:
                    throw new ArgumentException("IO Completion was neither a Send, Receive or an Accept operation.");
            }
        }

        private void StartReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            Session token = (Session)receiveEventArgs.UserToken;

            bool willRaiseEvent = token.Socket.ReceiveAsync(receiveEventArgs);

            if (!willRaiseEvent)
            {
                ProcessReceive(receiveEventArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            Session token = (Session)receiveEventArgs.UserToken;

            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                byte[] dataReceived = new byte[receiveEventArgs.BytesTransferred];
                Buffer.BlockCopy(receiveEventArgs.Buffer, receiveEventArgs.Offset, dataReceived, 0, receiveEventArgs.BytesTransferred);
                token.OnReceiveData(dataReceived);

                StartReceive(receiveEventArgs);
            }
            else
            {
                CloseClientSocket(receiveEventArgs);
                ReturnReceiveSaea(receiveEventArgs);
            }
        }

        public void BeginSend(SocketAsyncEventArgs sendEventArgs)
        {
            StartSend(sendEventArgs);
        }

        private void StartSend(SocketAsyncEventArgs sendEventArgs)
        {
            SendDataToken token = (SendDataToken)sendEventArgs.UserToken;

            if (token.SendBytesRemainingCount <= this.Settings.BufferSize)
            {
                sendEventArgs.SetBuffer(sendEventArgs.Offset, token.SendBytesRemainingCount);
                Buffer.BlockCopy(token.DataToSend, token.BytesSentAlreadyCount, sendEventArgs.Buffer, sendEventArgs.Offset, token.SendBytesRemainingCount);
            }
            else
            {
                sendEventArgs.SetBuffer(sendEventArgs.Offset, this.Settings.BufferSize);
                Buffer.BlockCopy(token.DataToSend, token.BytesSentAlreadyCount, sendEventArgs.Buffer, sendEventArgs.Offset, this.Settings.BufferSize);
            }

            bool willRaiseEvent = sendEventArgs.AcceptSocket.SendAsync(sendEventArgs);

            if (!willRaiseEvent)
            {
                ProcessSend(sendEventArgs);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            SendDataToken token = (SendDataToken)sendEventArgs.UserToken;

            if (sendEventArgs.SocketError == SocketError.Success)
            {
                token.SendBytesRemainingCount = token.SendBytesRemainingCount - sendEventArgs.BytesTransferred;

                if (token.SendBytesRemainingCount == 0)
                {
                    token.Reset();
                    ReturnSendSaea(sendEventArgs);
                }
                else
                {
                    token.BytesSentAlreadyCount += sendEventArgs.BytesTransferred;
                    StartSend(sendEventArgs);
                }
            }
            else
            {
                token.Reset();
                CloseClientSocket(sendEventArgs); // why???????
                ReturnSendSaea(sendEventArgs);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs args)
        {
            if (args.UserToken.GetType() != typeof(Session))
            {
                return;
            }

            Session con = (Session)args.UserToken;

            try
            {
                con.Socket.Disconnect(false);
            }
            catch (SocketException) { }

            con.OnDisconnection();

            log.Debug("<Session " + con.Id + "> is no longer in use.");
        }

        private void ReturnReceiveSaea(SocketAsyncEventArgs args)
        {
            this.PoolOfRecEventArgs.Push(args);
            this.MaxConnectionsEnforcer.Release();
        }

        private void ReturnSendSaea(SocketAsyncEventArgs args)
        {
            ((SendDataToken)args.UserToken).Session.OnSendCompleted();
        }

        private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            acceptEventArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
            acceptEventArgs.AcceptSocket.Close();
            this.PoolOfAcceptEventArgs.Push(acceptEventArgs);
        }

        [Obsolete]
        public void Shutdown()
        {
            this.ListenSocket.Shutdown(SocketShutdown.Both);
            this.ListenSocket.Close();

            DisposeAllSaeaObjects();
        }

        private void DisposeAllSaeaObjects()
        {
            this.PoolOfAcceptEventArgs.Dispose();
            this.PoolOfRecEventArgs.Dispose();
        }
    }
}
