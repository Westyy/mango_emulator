using System;
using System.Collections.Generic;

using Mango.Rooms.Mapping;
using Mango.Rooms;
using Mango.Items.Events;

namespace Mango.Items
{
    sealed class Item
    {
        private int _id;
        private ItemData _data;
        private int _dataId;
        private int _userId;
        private int _roomId;
        private string _roomWallPos;
        private int _roomRot;
        private Vector3D _position;
        private string _flags;
        private string _displayFlags;
        private string _initialFlags;
        private bool _untradable;
        private double _expireTimestamp;
        private int _soundManagerId;
        private int _soundManagerOrder;

        private Dictionary<int, RoomAvatar> _tmpInteractingAvatars;
        private bool _updateNeeded;
        private int _updateTicks;

        public Item(int Id, ItemData Data, int DataId, int UserId, int RoomId, string RoomWallPos, int RoomRot, Vector3D Position,
            string Flags, string DisplayFlags, bool Untradable, double ExpireTimestamp, int SoundManagerId,
            int SoundManagerOrder)
        {
            this.Id = Id;
            this.Data = Data;
            this.DataId = DataId;
            this.UserId = UserId;
            this.RoomId = RoomId;
            this.RoomWallPos = RoomWallPos;
            this.RoomRot = RoomRot;
            this.Position = Position;
            this.Flags = Flags;
            this.DisplayFlags = DisplayFlags;
            this.InitialFlags = Flags;
            this.Untradable = Untradable;
            this.ExpireTimestamp = ExpireTimestamp;
            this.SoundManagerId = SoundManagerId;
            this.SoundManagerOrder = SoundManagerOrder;

            this.UpdateNeeded = false;
            this.UpdateTicks = 0;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public ItemData Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        public int DataId
        {
            get { return this._dataId; }
            set { this._dataId = value; }
        }

        public int UserId
        {
            get { return this._userId; }
            set { this._userId = value; }
        }

        public int RoomId
        {
            get { return this._roomId; }
            set { this._roomId = value; }
        }

        public string RoomWallPos
        {
            get { return this._roomWallPos; }
            set { this._roomWallPos = value; }
        }

        public int RoomRot
        {
            get { return this._roomRot; }
            set { this._roomRot = value; }
        }

        public Vector3D Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public string Flags
        {
            get { return this._flags; }
            set { this._flags = value; }
        }

        public string DisplayFlags
        {
            get { return this._displayFlags; }
            set { this._displayFlags = value; }
        }

        public string InitialFlags
        {
            get { return this._initialFlags; }
            set { this._initialFlags = value; }
        }

        public bool Untradable
        {
            get { return this._untradable; }
            set { this._untradable = value; }
        }

        public double ExpireTimestamp
        {
            get { return this._expireTimestamp; }
            set { this._expireTimestamp = value; }
        }

        public int SoundManagerId
        {
            get { return this._soundManagerId; }
            set { this._soundManagerId = value; }
        }

        public int SoundManagerOrder
        {
            get { return this._soundManagerOrder; }
            set { this._soundManagerOrder = value; }
        }

        public Dictionary<int, RoomAvatar> TmpInteractingAvatars
        {
            get
            {
                if (this._tmpInteractingAvatars == null) { this._tmpInteractingAvatars = new Dictionary<int, RoomAvatar>(3); }
                return this._tmpInteractingAvatars;
            }
        }

        public bool UpdateNeeded
        {
            get { return this._updateNeeded; }
            set { this._updateNeeded = value; }
        }

        public int UpdateTicks
        {
            get { return this._updateTicks; }
            set { this._updateTicks = value; }
        }

        /// <summary>
        /// Returns the next square in front.
        /// </summary>
        public Vector2D SquareInFront
        {
            get
            {
                if (!InRoom) { throw new InvalidOperationException("Invalid call to method, item is not in a room."); }

                Vector2D PosNow = new Vector2D(this.Position.X, this.Position.Y);

                if (this.RoomRot == 0)
                {
                    PosNow.Y--;
                }
                else if (this.RoomRot == 2)
                {
                    PosNow.X++;
                }
                else if (this.RoomRot == 4)
                {
                    PosNow.Y++;
                }
                else if (this.RoomRot == 6)
                {
                    PosNow.X--;
                }

                return PosNow;
            }
        }

        /// <summary>
        /// Returns the next square behind.
        /// </summary>
        public Vector2D SquareBehind
        {
            get
            {
                if (!InRoom) { throw new InvalidOperationException("Invalid call to method, item is not in a room."); }

                Vector2D PosNow = new Vector2D(this.Position.X, this.Position.Y);

                if (this.RoomRot == 0)
                {
                    PosNow.Y++;
                }
                else if (this.RoomRot == 2)
                {
                    PosNow.X--;
                }
                else if (this.RoomRot == 4)
                {
                    PosNow.Y--;
                }
                else if (this.RoomRot == 6)
                {
                    PosNow.X++;
                }

                return PosNow;
            }
        }

        /// <summary>
        /// Cycles the tick count for this item.
        /// </summary>
        /// <param name="Instance"></param>
        public void Update(RoomInstance Instance)
        {
            if (!this.UpdateNeeded)
            {
                return;
            }

            if (this.UpdateTicks > 0)
            {
                UpdateTicks--;
            }

            if (this.UpdateTicks < 0)
            {
                this.UpdateTicks = 0;
            }

            if (this.UpdateNeeded && this.UpdateTicks == 0)
            {
                this.UpdateNeeded = false;
                Mango.GetServer().GetItemEventManager().Handle(null, this, ItemEventType.UpdateTick, Instance);
            }
        }

        /// <summary>
        /// Requests an update.
        /// </summary>
        /// <param name="UpdateTicks">Amount of ticks, every 2 ticks are 1 second. (2 = 1s, 4 = 2s)</param>
        public void RequestUpdate(int UpdateTicks)
        {
            this.UpdateNeeded = true;
            this.UpdateTicks = UpdateTicks;
        }

        /// <summary>
        /// Is this item placed in a room.
        /// </summary>
        public bool InRoom
        {
            get
            {
                return (this.RoomId > 0);
            }
        }

        /// <summary>
        /// Item pending expiration.
        /// </summary>
        [Obsolete("Removed from Habbo.")]
        public bool PendingExpiration
        {
            get
            {
                return (this.ExpireTimestamp > 0);
            }
        }

        [Obsolete("Removed from Habbo.")]
        public bool Expires
        {
            get
            {
                return (this.ExpireTimestamp > 0);
            }
        }

        [Obsolete("Removed from Habbo.")]
        public bool Expired
        {
            get
            {
                return false;
            }
        }

        [Obsolete("Removed from Habbo.")]
        public double ExpireTimeLeft
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Is this item usable?
        /// </summary>
        public bool Usable
        {
            get
            {
                return false; // needs to be coded or looked into see what it does etc
            }
        }


    }
}
