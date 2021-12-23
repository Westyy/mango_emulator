using Mango.Players;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Logs
{
    sealed class ChatlogEntry
    {
        private int _playerId;
        private int _roomId;
        private string _message;
        private double _timestamp;

        private WeakReference _playerReference;
        private WeakReference _roomReference;

        public ChatlogEntry(int PlayerId, int RoomId, string Message, double Timestamp, Player Player = null, RoomInstance Instance = null)
        {
            this._playerId = PlayerId;
            this._roomId = RoomId;
            this._message = Message;
            this._timestamp = Timestamp;

            if (Player != null)
            {
                this._playerReference = new WeakReference(Player);
            }

            if (Instance != null)
            {
                this._roomReference = new WeakReference(Instance);
            }
        }

        public int PlayerId
        {
            get { return this._playerId; }
        }

        public int RoomId
        {
            get { return this._roomId; }
        }

        public string Message
        {
            get { return this._message; }
        }

        public double Timestamp
        {
            get { return this._timestamp; }
        }

        public Player PlayerNullable()
        {
            if (this._playerReference.IsAlive)
            {
                Player PlayerObj = (Player)this._playerReference.Target;

                if (PlayerObj.IsDisposed)
                {
                    return null;
                }

                return PlayerObj;
            }

            return null;
        }

        public RoomInstance RoomNullable()
        {
            if (this._roomReference.IsAlive)
            {
                RoomInstance RoomObj = (RoomInstance)this._roomReference.Target;

                if (RoomObj.Unloaded)
                {
                    return null;
                }

                return RoomObj;
            }

            return null;
        }
    }
}
