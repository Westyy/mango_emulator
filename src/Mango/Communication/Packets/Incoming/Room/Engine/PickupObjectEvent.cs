using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class PickupObjectEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!session.GetPlayer().GetAvatar().InRoom || !instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            int Unknown1 = packet.PopWiredInt();
            int ItemId = packet.PopWiredInt();

            Item Item = null;

            if (!instance.GetItems().TryGetItem(ItemId, out Item))
            {
                return;
            }

            if (Item.Data.Type != ItemType.FLOOR && Item.Data.Type != ItemType.WALL)
            {
                return;
            }

            if (!instance.GetItems().TryTakeItem(Item))
            {
                return;
            }

            if (session.GetPlayer().GetInventory().TryAddItem(Item))
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("UPDATE `items` SET `room_pos_x` = @x, `room_pos_y` = @y, `room_pos_z` = @z, `room_rot` = @rot, `user_id` = @uid, `room_id` = @rid, `room_wall_pos` = @wallpos WHERE `id` = @id LIMIT 1;");
                        DbCon.AddParameter("x", 0);
                        DbCon.AddParameter("y", 0);
                        DbCon.AddParameter("z", 0);
                        DbCon.AddParameter("rot", 0);
                        DbCon.AddParameter("uid", session.GetPlayer().Id);
                        DbCon.AddParameter("rid", 0);
                        DbCon.AddParameter("wallpos", "");
                        DbCon.AddParameter("id", Item.Id);
                        DbCon.ExecuteNonQuery();

                        Item.Position.X = 0;
                        Item.Position.Y = 0;
                        Item.Position.Z = 0;
                        Item.RoomRot = 0;
                        Item.UserId = session.GetPlayer().Id;
                        Item.RoomId = 0;
                        Item.RoomWallPos = "";

                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                        instance.GetItems().TryAddItem(Item);
                    }
                }

                if (Item.Data.Type == ItemType.FLOOR)
                {
                    instance.GetAvatars().BroadcastPacket(new ObjectRemoveComposer(Item, session.GetPlayer().Id));
                }
                else if (Item.Data.Type == ItemType.WALL)
                {
                    instance.GetAvatars().BroadcastPacket(new ItemRemoveComposer(Item, session.GetPlayer().Id));
                }

                instance.GetMapping().RegenerateRelativeHeightmap();
                session.SendPacket(new FurniListInsertComposer(Item));
            }
            else
            {
                instance.GetItems().TryAddItem(Item); // attempt to restore it
            }
        }
    }
}
