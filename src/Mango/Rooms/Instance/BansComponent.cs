using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Mango.Players;
using Mango.Utilities;

namespace Mango.Rooms.Instance
{
    class BansComponent
    {
        /// <summary>
        /// The RoomInstance that created this BanComponent.
        /// </summary>
        private RoomInstance Instance = null;

        /// <summary>
        /// The bans collection for storing them for this room.
        /// </summary>
        private ConcurrentDictionary<int, double> _bans;

        /// <summary>
        /// Create the BanComponent for the RoomInstance.
        /// </summary>
        /// <param name="instance">The instance that created this component.</param>
        public BansComponent(RoomInstance instance)
        {
            if (instance == null) throw new NullReferenceException("RoomInstance cannot be null");

            this.Instance = instance;

            this._bans = new ConcurrentDictionary<int,double>();
        }

        public void Ban(RoomAvatar Avatar)
        {
            if (Avatar == null) { throw new NullReferenceException("Avatar cannot be null."); }

            if (Instance.GetRights().CheckRights(Avatar, true))
            {
                return;
            }

            double BanTime = UnixTimestamp.GetNow();

            if (!this._bans.TryAdd(Avatar.Player.Id, BanTime))
            {
                this._bans[Avatar.Player.Id] = BanTime;
            }

            this.Instance.GetAvatars().SoftKickAvatar(Avatar, true, true);
        }

        public bool IsBanned(RoomAvatar Avatar)
        {
            if (!this._bans.ContainsKey(Avatar.Player.Id))
            {
                return false;
            }

            double BanTime = UnixTimestamp.GetNow() - this._bans[Avatar.Player.Id];

            if (BanTime >= 900)
            {
                double time;
                this._bans.TryRemove(Avatar.Player.Id, out time);
                return false;
            }

            return true;
        }

        public void ClearBans()
        {
            this._bans.Clear();
        }

        public void Cleanup()
        {
            this._bans.Clear();

            this.Instance = null;
            this._bans = null;
        }
    }
}
