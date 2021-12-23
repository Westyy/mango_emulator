using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Room.Permissions;
using MySql.Data.MySqlClient;

namespace Mango.Rooms.Instance
{
    class RightsComponent
    {
        private RoomInstance Instance = null;

        public RightsComponent(RoomInstance Instance)
        {
            if (Instance == null) { throw new NullReferenceException("RoomInstance cannot be null."); }

            this.Instance = Instance;
        }

        private IList<int> UsersWithRights
        {
            get { return this.Instance.UsersWithRights; }
        }

        public bool CheckRights(RoomAvatar Avatar, bool requireOwnership = false)
        {
            bool IsOwner = (Avatar.Player.GetPermissions().HasRight("room_any_owner") || Avatar.Player.Id == this.Instance.OwnerId);

            if (requireOwnership)
            {
                return IsOwner;
            }

            return (IsOwner || Avatar.Player.GetPermissions().HasRight("room_any_rights") || this.UsersWithRights.Contains(Avatar.Player.Id));
        }

        public bool GiveRights(RoomAvatar Avatar)
        {
            if (this.UsersWithRights.Contains(Avatar.Player.Id))
            {
                return false;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.SetQuery("INSERT INTO `room_rights` (room_id,user_id) VALUES(@rid,@uid);");
                    DbCon.AddParameter("rid", this.Instance.Id);
                    DbCon.AddParameter("uid", Avatar.Player.Id);
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return false;
                }
            }

            this.UsersWithRights.Add(Avatar.Player.Id);

            Avatar.SetStatus("flatctrl");
            Avatar.UpdateNeeded = true;

            Avatar.Player.GetSession().SendPacket(new YouAreControllerComposer());

            return true;
        }

        public bool TakeRights(int Id)
        {
            if (!this.UsersWithRights.Contains(Id))
            {
                return false;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid;");
                    DbCon.AddParameter("rid", this.Instance.Id);
                    DbCon.AddParameter("uid", Id);
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return false;
                }
            }

            this.UsersWithRights.Remove(Id);

            RoomAvatar Avatar = null;

            if (this.Instance.GetAvatars().TryGet(Id, out Avatar))
            {
                Avatar.RemoveStatus("flatctrl");
                Avatar.UpdateNeeded = true;

                Avatar.Player.GetSession().SendPacket(new YouAreNotControllerComposer());
            }

            return true;
        }

        public void Cleanup()
        {
            this.Instance = null;
        }
    }
}
