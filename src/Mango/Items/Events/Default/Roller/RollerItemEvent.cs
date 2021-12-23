using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Rooms.Mapping;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Roller
{
    class RollerItemEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            switch (Type)
            {
                case ItemEventType.UpdateTick:
                    if (Instance.GetAvatars().Count == 0)
                    {
                        Item.RequestUpdate(4);
                        break;
                    }

                    List<RoomAvatar> AvatarsOnPosition = Instance.GetMapping().GetAvatarsOnPosition(Item.Position.ToVector2D());
                    List<Item> ItemsToMove = new List<Item>();
                    ItemsToMove.AddRange(Instance.GetItems().GetItemsOnPosition(Item.Position.ToVector2D()));

                    if (AvatarsOnPosition != null)
                    {
                        foreach (RoomAvatar Avatar in AvatarsOnPosition)
                        {
                            if (Avatar.IsMoving || Avatar.PositionToSet != null)
                            {
                                continue;
                            }

                            Vector2D FinalPosition = Instance.GetMapping().GetRedirectedTarget(Item.SquareInFront);

                            if (Instance.GetMapping().IsValidStep(Avatar.Position.ToVector2D(), FinalPosition, true))
                            {
                                if (Avatar.ForceSit)
                                {
                                    Avatar.ForceSit = false;
                                }

                                Avatar.PositionToSet = FinalPosition;
                                Instance.GetAvatars().BroadcastPacket(new SlideObjectBundleComposer(Avatar.Position, new Vector3D(Avatar.PositionToSet.X, Avatar.PositionToSet.Y, Instance.GetMapping().GetUserStepHeight(Avatar.PositionToSet)), Item.Id, Avatar.Player.Id, 0));
                            }
                        }
                    }

                    if (ItemsToMove.Count > 0)
                    {
                        foreach (Item ItemMove in ItemsToMove)
                        {
                            if (Instance.GetItems().IsItemRolledAlready(ItemMove))
                            {
                                continue;
                            }

                            if (ItemMove.Data.Behaviour == ItemBehaviour.ROLLER)
                            {
                                continue;
                            }

                            if (ItemMove == Item)
                            {
                                continue;
                            }

                            if ((Item.Position.X == ItemMove.Position.X && Item.Position.Y == ItemMove.Position.Y) && !Instance.GetMapping().IsTargetBlocked(Item.SquareInFront))
                            {
                                Vector2D MoveTo = new Vector2D(Item.SquareInFront.X, Item.SquareInFront.Y);
                                int MoveToRot = ItemMove.RoomRot;
                                Vector3D OldPosition = new Vector3D(ItemMove.Position.X, ItemMove.Position.Y, ItemMove.Position.Z);

                                Vector3D Position = null;

                                if (Instance.GetItems().TrySetFloorItem(null, ItemMove, MoveTo.X, MoveTo.Y, MoveToRot, out Position))
                                {
                                    using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                                    {
                                        try
                                        {
                                            DbCon.Open();
                                            DbCon.BeginTransaction();

                                            DbCon.SetQuery("UPDATE `items` SET `room_pos_x` = @x, `room_pos_y` = @y, `room_pos_z` = @z, `room_rot` = @rot WHERE `id` = @id;");
                                            DbCon.AddParameter("x", Position.X);
                                            DbCon.AddParameter("y", Position.Y);
                                            DbCon.AddParameter("z", Position.Z);
                                            DbCon.AddParameter("rot", MoveToRot);
                                            DbCon.AddParameter("id", ItemMove.Id);
                                            DbCon.ExecuteNonQuery();

                                            ItemMove.Position.X = Position.X;
                                            ItemMove.Position.Y = Position.Y;
                                            ItemMove.Position.Z = Position.Z;
                                            ItemMove.RoomRot = MoveToRot;

                                            DbCon.Commit();
                                        }
                                        catch (MySqlException)
                                        {
                                            DbCon.Rollback();
                                            return;
                                        }
                                    }

                                    Instance.GetItems().SetItemJustRolled(ItemMove);

                                    Instance.GetMapping().RegenerateRelativeHeightmap();
                                    //Instance.GetAvatars().BroadcastPacket(new ObjectUpdateComposer(ItemMove, 0));

                                    Mango.GetServer().GetItemEventManager().Handle(Session, ItemMove, ItemEventType.Moved, Instance);
                                    Instance.GetAvatars().BroadcastPacket(new SlideObjectBundleComposer(OldPosition, Position, Item.Id, 0, ItemMove.Id));
                                }
                            }
                        }
                    }

                    goto case ItemEventType.InstanceLoaded;

                case ItemEventType.InstanceLoaded:
                case ItemEventType.Placed:
                    Item.RequestUpdate(4);
                    break;
            }
        }
    }
}
