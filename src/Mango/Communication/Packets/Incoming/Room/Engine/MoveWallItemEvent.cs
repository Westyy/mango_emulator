using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class MoveWallItemEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance Instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (!Instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            int ItemId = packet.PopWiredInt();
            string RawPlacementData = packet.PopString();

            Item Item = null;

            if (!Instance.GetItems().TryGetWallItem(ItemId, out Item))
            {
                return;
            }

            if (Item.Data.Type != ItemType.WALL)
            {
                return;
            }

            string WallPos = string.Empty;

            if (Instance.GetItems().TrySetWallItem(session.GetPlayer(), Item, RawPlacementData, out WallPos))
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("UPDATE `items` SET `room_pos_x` = @x, `room_pos_y` = @y, `room_pos_z` = @z, `room_rot` = @rot, `user_id` = @uid, `room_id` = @rid, `room_wall_pos` = @wallpos WHERE `id` = @id;");
                        DbCon.AddParameter("x", 0);
                        DbCon.AddParameter("y", 0);
                        DbCon.AddParameter("z", 0);
                        DbCon.AddParameter("rot", 0);
                        DbCon.AddParameter("uid", 0);
                        DbCon.AddParameter("rid", Instance.Id);
                        DbCon.AddParameter("wallpos", WallPos);
                        DbCon.AddParameter("id", Item.Id);
                        DbCon.ExecuteNonQuery();

                        Item.Position.X = 0;
                        Item.Position.Y = 0;
                        Item.Position.Z = 0;
                        Item.RoomRot = 0;
                        Item.UserId = 0;
                        Item.RoomId = Instance.Id;
                        Item.RoomWallPos = WallPos;

                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }

                Instance.GetAvatars().BroadcastPacket(new ItemUpdateComposer(Item, Instance.OwnerId));
            }
        }
    }
}
