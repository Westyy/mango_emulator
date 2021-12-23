using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Players.Wardrobe;
using Mango.Communication.Packets.Outgoing.Moderation;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Avatar
{
    class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int SlotsAvailable = 0;

            if (Session.GetPlayer().GetPermissions().HasRight("club_regular"))
            {
                SlotsAvailable = 5;
            }

            if (Session.GetPlayer().GetPermissions().HasRight("club_vip"))
            {
                SlotsAvailable = 10;
            }

            if (SlotsAvailable == 0)
            {
                return;
            }

            int SlotId = Packet.PopWiredInt();
            string Figure = Packet.PopString();
            PlayerGender Gender = (Packet.PopString().ToLower() == "m" ? PlayerGender.MALE : PlayerGender.FEMALE);

            if (SlotId <= 0 || SlotId > MangoStaticSettings.WardrobeClientMaxSlots)
            {
                return;
            }

            if (!FigureValidation.Validate(Figure))
            {
                Session.SendPacket(new ModMessageComposer("Error changing figure."));
                return;
            }

            WardrobeItem Item = null;

            if (Session.GetPlayer().GetWardrobe().TryGet(SlotId, out Item))
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("UPDATE `wardrobe` SET `figure` = @fig, `gender` = @gen WHERE `id` = @id LIMIT 1;");
                        DbCon.AddParameter("fig", Figure);
                        DbCon.AddParameter("gen", Gender == PlayerGender.MALE ? "M" : "F");
                        DbCon.AddParameter("id", Item.Id);
                        DbCon.ExecuteNonQuery();

                        Item.Figure = Figure;
                        Item.Gender = Gender;

                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }
            }
            else
            {
                WardrobeItem NewItem = new WardrobeItem(0, Session.GetPlayer().Id, SlotId, Figure, Gender == PlayerGender.MALE ? "m" : "f");

                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("INSERT INTO `wardrobe` (user_id,slot_id,figure,gender) VALUES(@uid,@sid,@fig,@gen);");
                        DbCon.AddParameter("fig", NewItem.Figure);
                        DbCon.AddParameter("gen", NewItem.Gender == PlayerGender.MALE ? "M" : "F");
                        DbCon.AddParameter("uid", NewItem.UserId);
                        DbCon.AddParameter("sid", NewItem.SlotId);
                        DbCon.ExecuteNonQuery();

                        NewItem.Id = DbCon.SelectLastId();

                        if (Session.GetPlayer().GetWardrobe().TryAdd(SlotId, NewItem))
                        {
                            DbCon.Commit();
                        }
                        else
                        {
                            DbCon.Rollback();
                        }
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }
            }
        }
    }
}
