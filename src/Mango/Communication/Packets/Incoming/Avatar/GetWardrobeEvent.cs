using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Players.Wardrobe;
using Mango.Communication.Packets.Outgoing.Avatar;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Avatar
{
    class GetWardrobeEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int MaximumSlots = 10; // static value of how many total slots are IN habbo client

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

            IDictionary<int, WardrobeItem> Items = new Dictionary<int, WardrobeItem>(Session.GetPlayer().GetWardrobe().WardobeItems);

            Dictionary<int, WardrobeItem> ValidItems = new Dictionary<int, WardrobeItem>();

            foreach (KeyValuePair<int, WardrobeItem> Item in Items)
            {
                if (Item.Key > SlotsAvailable)
                {
                    continue;
                }

                if (Item.Key > MaximumSlots) // self cleanup
                {
                    using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                    {
                        try
                        {
                            DbCon.Open();
                            DbCon.BeginTransaction();

                            DbCon.SetQuery("DELETE FROM `wardrobe` WHERE `id` = @id LIMIT 1;");
                            DbCon.AddParameter("id", Item.Value.Id);
                            DbCon.ExecuteNonQuery();

                            Session.GetPlayer().GetWardrobe().WardobeItems.Remove(Item.Key);

                            DbCon.Commit();
                        }
                        catch (MySqlException)
                        {
                            DbCon.Rollback();
                        }
                    }

                    continue;
                }

                ValidItems.Add(Item.Key, Item.Value);
            }

            Session.SendPacket(new WardrobeComposer(ValidItems));
        }
    }
}
