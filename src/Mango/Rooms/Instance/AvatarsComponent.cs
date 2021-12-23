using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Mango.Communication.Packets.Outgoing;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Room.Permissions;
using Mango.Communication.Packets.Outgoing.Room.Chat;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Handshake;

namespace Mango.Rooms.Instance
{
    sealed class AvatarsComponent
    {
        private RoomInstance Instance;

        private ConcurrentDictionary<int, RoomAvatar> _avatars;
        private ConcurrentDictionary<string, int> _avatarsNamesToId;

        public AvatarsComponent(RoomInstance Instance)
        {
            if (Instance == null) throw new NullReferenceException("RoomInstance cannot be null");

            this.Instance = Instance;

            this._avatars = new ConcurrentDictionary<int, RoomAvatar>();
            this._avatarsNamesToId = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        /// Avatars count.
        /// </summary>
        public int Count
        {
            get { return this._avatars.Count; }
        }

        /// <summary>
        /// RoomAvatar in the Avatars list.
        /// </summary>
        public ICollection<RoomAvatar> Avatars
        {
            get { return this._avatars.Values; }
        }

        public bool Contains(int AvatarId)
        {
            return this._avatars.ContainsKey(AvatarId);
        }

        public bool AddAvatarToRoom(RoomAvatar Avatar)
        {
            if (Avatar == null) throw new NullReferenceException("RoomAvatar cannot be null.");

            if (Avatar.GetCurrentRoom() == null || Avatar.CurrentRoomId != this.Instance.Id) // room checks
            {
                return false;
            }

            // set avatar position and rotation to the door
            Avatar.Position.X = Instance.Model.DoorX;
            Avatar.Position.Y = Instance.Model.DoorY;
            Avatar.Position.Z = Instance.Model.DoorZ;
            Avatar.BodyRotation = Instance.Model.DoorRotation;
            Avatar.HeadRotation = Instance.Model.DoorRotation;

            // teleporting stuff goes here ?

            if (!this._avatars.TryAdd(Avatar.Player.Id, Avatar))
            {
                return false;
            }

            if (!this._avatarsNamesToId.TryAdd(Avatar.Player.Username.ToLower(), Avatar.Player.Id))
            {
                RoomAvatar TakenAv = null;
                this._avatars.TryRemove(Avatar.Player.Id, out TakenAv);
                return false;
            }

            this.BroadcastPacket(new UsersComposer(Avatar));

            if (this.Instance.GetRights().CheckRights(Avatar, true))
            {
                Avatar.SetStatus("flatctrl", "useradmin");
                Avatar.Player.GetSession().SendPacket(new YouAreOwnerComposer());
                Avatar.Player.GetSession().SendPacket(new YouAreControllerComposer());
            }
            else if (this.Instance.GetRights().CheckRights(Avatar, false))
            {
                Avatar.SetStatus("flatctrl");
                Avatar.Player.GetSession().SendPacket(new YouAreControllerComposer());
            }

            Avatar.UpdateNeeded = true;

            return true;
        }

        /// <summary>
        /// Removes the Avatar from this room instance.
        /// </summary>
        /// <param name="Avatar"></param>
        /// <returns></returns>
        public bool RemoveAvatarFromRoom(RoomAvatar Avatar)
        {
            RoomAvatar Taken = null;
            int TakenId = 0;

            if (!this._avatars.TryRemove(Avatar.Player.Id, out Taken))
            {
                return false;
            }

            if (!this._avatarsNamesToId.TryRemove(Avatar.Player.Username.ToLower(), out TakenId))
            {
                return false;
            }

            // stop active avatar trades

            BroadcastPacket(new UserRemoveComposer(Avatar.Player.Id));

            return true;
        }

        /// <summary>
        /// Kicks the Avatar from this room instance.
        /// </summary>
        /// <param name="Avatar"></param>
        /// <param name="Forced"></param>
        /// <param name="NotifyUser"></param>
        /// <param name="OverrideOwner"></param>
        public void SoftKickAvatar(RoomAvatar Avatar, bool Forced = false, bool NotifyUser = false, bool OverrideOwner = false)
        {
            if (Avatar.CurrentRoomId != this.Instance.Id)
            {
                return;
            }

            if (!OverrideOwner && this.Instance.GetRights().CheckRights(Avatar))
            {
                return; // this is the room owner or a moderator, no kicking allowed!
            }

            if (NotifyUser)
            {
                Avatar.Player.GetSession().SendPacket(new GenericErrorComposer(4008));
            }

            Avatar.LeaveRoom(Forced);
        }

        /// <summary>
        /// Broadcasts a ServerPacket to all Avatars in this room.
        /// </summary>
        /// <param name="Packet"></param>
        /// <param name="UsersWithRightsOnly"></param>
        public void BroadcastPacket(ServerPacket Packet, bool UsersWithRightsOnly = false)
        {
            foreach (RoomAvatar Avatar in this._avatars.Values)
            {
                if (Avatar.Type != RoomAvatarType.Player)
                {
                    continue;
                }

                if (UsersWithRightsOnly && !Instance.GetRights().CheckRights(Avatar))
                {
                    continue;
                }

                Avatar.Player.GetSession().SendPacket(Packet);
            }
        }

        public bool TryGet(int Id, out RoomAvatar Avatar)
        {
            return this._avatars.TryGetValue(Id, out Avatar);
        }

        public bool TryGet(string Username, out RoomAvatar Avatar)
        {
            // All in lowercase
            Username = Username.ToLower();

            int Id;

            if (this._avatarsNamesToId.TryGetValue(Username, out Id))
            {
                return this._avatars.TryGetValue(Id, out Avatar);
            }
            else
            {
                Avatar = null;
                return false;
            }
        }

        public void RemoveAllGracefully(bool Forced = false)
        {
            foreach (RoomAvatar Avatar in this._avatars.Values)
            {
                Avatar.LeaveRoom(Forced);
            }
        }

        public void RemoveAll()
        {
            foreach (RoomAvatar avatar in this._avatars.Values)
            {
                avatar.ExitRoom();
            }
        }

        public void Cleanup()
        {
            this._avatars.Clear();

            this.Instance = null;
            this._avatars = null;
        }
    }
}
