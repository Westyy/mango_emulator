using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Items;
using Mango.Rooms.Mapping;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class MoveObjectEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Session.GetPlayer().GetAvatar().InRoom || !Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()))
            {
                return;
            }

            Item Item = null;

            if (Instance.GetItems().TryGetFloorItem(Packet.PopWiredInt(), out Item))
            {
                if (Item.Data.Type != ItemType.FLOOR)
                {
                    return;
                }

                int newX = Packet.PopWiredInt();
                int newY = Packet.PopWiredInt();
                int newRot = Packet.PopWiredInt();

                Vector3D NewPosition = null;

                if (Instance.GetItems().TrySetFloorItem(Session.GetPlayer(), Item, newX, newY, newRot, out NewPosition))
                {
                    using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                    {
                        try
                        {
                            DbCon.Open();
                            DbCon.BeginTransaction();

                            DbCon.SetQuery("UPDATE `items` SET `room_pos_x` = @x, `room_pos_y` = @y, `room_pos_z` = @z, `room_rot` = @rot WHERE `id` = @id;");
                            DbCon.AddParameter("x", NewPosition.X);
                            DbCon.AddParameter("y", NewPosition.Y);
                            DbCon.AddParameter("z", NewPosition.Z);
                            DbCon.AddParameter("rot", newRot);
                            DbCon.AddParameter("id", Item.Id);
                            DbCon.ExecuteNonQuery();

                            Item.Position.X = NewPosition.X;
                            Item.Position.Y = NewPosition.Y;
                            Item.Position.Z = NewPosition.Z;
                            Item.RoomRot = newRot;

                            DbCon.Commit();
                        }
                        catch (MySqlException)
                        {
                            DbCon.Rollback();
                            return;
                        }
                    }

                    Instance.GetMapping().RegenerateRelativeHeightmap();
                    //Instance.GetAvatars().BroadcastPacket(new ObjectUpdateComposer(Item, Session.GetPlayer().Id));
                }

                Instance.GetAvatars().BroadcastPacket(new ObjectUpdateComposer(Item, Session.GetPlayer().Id));
            }
        }
    }
}
