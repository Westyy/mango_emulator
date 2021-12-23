using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Rooms.Mapping;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;
using Mango.Items.Events;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class PlaceObjectEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            int ItemId = 0;
            string[] Data = null;

            string RawData = packet.PopString();
            Data = RawData.Split(' ');

            if (!int.TryParse(Data[0], out ItemId))
            {
                return;
            }

            Item Item = null;

            if (!session.GetPlayer().GetInventory().TryGetItem(ItemId, out Item))
            {
                return;
            }

            bool HasPlacementRights = instance.GetRights().CheckRights(session.GetPlayer().GetAvatar());
            bool IsPlacingGuestSticky = false;

            if (Item.Data.Behaviour == ItemBehaviour.STICKY_NOTE && !HasPlacementRights && instance.GetItems().GuestsCanPlaceStickies)
            {
                IsPlacingGuestSticky = true;
            }
            else if (!HasPlacementRights)
            {
                session.SendPacket(new PlaceObjectErrorComposer(ItemPlacementError.INSUFFICIENT_RIGHTS));
                return;
            }

            if (Item.PendingExpiration && Item.ExpireTimeLeft < 0)
            {
                // to-do: remove item
                return;
            }

            switch (Item.Data.Type)
            {
                default:
                case ItemType.FLOOR:

                    if (Data.Length < 4)
                    {
                        return;
                    }

                    int X = 0;
                    int Y = 0;
                    int Rotation = 0;

                    if (!int.TryParse(Data[1], out X)) { return; }
                    if (!int.TryParse(Data[2], out Y)) { return; }
                    if (!int.TryParse(Data[3], out Rotation)) { return; }

                    Vector3D NewPosition = null;

                    Item RemovedFloorItem = null;

                    if (!session.GetPlayer().GetInventory().TryRemoveItem(ItemId, out RemovedFloorItem))
                    {
                        return;
                    }

                    if (instance.GetItems().TrySetFloorItem(session.GetPlayer(), Item, X, Y, Rotation, out NewPosition))
                    {
                        using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                        {
                            try
                            {
                                DbCon.Open();
                                DbCon.BeginTransaction();

                                DbCon.SetQuery("UPDATE `items` SET `room_pos_x` = @x, `room_pos_y` = @y, `room_pos_z` = @z, `room_rot` = @rot, `user_id` = @uid, `room_id` = @rid, `room_wall_pos` = @wallpos WHERE `id` = @id;");
                                DbCon.AddParameter("x", NewPosition.X);
                                DbCon.AddParameter("y", NewPosition.Y);
                                DbCon.AddParameter("z", NewPosition.Z);
                                DbCon.AddParameter("rot", Rotation);
                                DbCon.AddParameter("uid", 0);
                                DbCon.AddParameter("rid", instance.Id);
                                DbCon.AddParameter("wallpos", "");
                                DbCon.AddParameter("id", Item.Id);
                                DbCon.ExecuteNonQuery();

                                Item.Position.X = NewPosition.X;
                                Item.Position.Y = NewPosition.Y;
                                Item.Position.Z = NewPosition.Z;
                                Item.RoomRot = Rotation;
                                Item.UserId = 0;
                                Item.RoomId = instance.Id;
                                Item.RoomWallPos = "";

                                DbCon.Commit();
                            }
                            catch (MySqlException)
                            {
                                DbCon.Rollback();
                                session.GetPlayer().GetInventory().TryAddFloorItem(Item);
                                break;
                            }
                        }

                        // to-do: we need to handle quests etc for this floor item and item handles (on place item).
                        session.SendPacket(new FurniListRemoveComposer(Item));
                        instance.GetAvatars().BroadcastPacket(new ObjectAddComposer(Item, instance));

                        Mango.GetServer().GetItemEventManager().Handle(session, Item, ItemEventType.Placed, session.GetPlayer().GetAvatar().GetCurrentRoom());

                        instance.GetMapping().RegenerateRelativeHeightmap();
                    }
                    else
                    {
                        session.GetPlayer().GetInventory().TryAddFloorItem(Item);
                    }

                    break;

                case ItemType.WALL:

                    string[] CorrectedData = new string[Data.Length - 1];

                    for (int i = 1; i < Data.Length; i++)
                    {
                        CorrectedData[i - 1] = Data[i];
                    }

                    string WallPos = string.Empty;

                    Item RemovedWallItem = null;

                    if (!session.GetPlayer().GetInventory().TryRemoveItem(ItemId, out RemovedWallItem))
                    {
                        return;
                    }

                    if (instance.GetItems().TrySetWallItem(session.GetPlayer(), Item, CorrectedData, out WallPos))
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
                                DbCon.AddParameter("rid", instance.Id);
                                DbCon.AddParameter("wallpos", WallPos);
                                DbCon.AddParameter("id", Item.Id);
                                DbCon.ExecuteNonQuery();

                                Item.Position.X = 0;
                                Item.Position.Y = 0;
                                Item.Position.Z = 0;
                                Item.RoomRot = 0;
                                Item.UserId = 0;
                                Item.RoomId = instance.Id;
                                Item.RoomWallPos = WallPos;

                                DbCon.Commit();
                            }
                            catch (MySqlException)
                            {
                                DbCon.Rollback();
                                session.GetPlayer().GetInventory().TryAddWallItem(Item);
                            }
                        }

                        instance.GetAvatars().BroadcastPacket(new ItemAddComposer(Item, session.GetPlayer().Username));
                        session.SendPacket(new FurniListRemoveComposer(Item));

                        Mango.GetServer().GetItemEventManager().Handle(session, Item, ItemEventType.Placed, session.GetPlayer().GetAvatar().GetCurrentRoom());

                        if (IsPlacingGuestSticky)
                        {
                            instance.GetItems().GiveTemporaryStickyEditingRights(Item.Id, session.GetPlayer().Id);
                            session.SendPacket(new ItemDataUpdateComposer(Item));
                        }
                    }
                    else
                    {
                        session.GetPlayer().GetInventory().TryAddWallItem(Item);
                    }

                    break;
            }
        }
    }
}
