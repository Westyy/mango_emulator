using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class ApplyDecorationEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (!session.GetPlayer().GetAvatar().GetCurrentRoom().GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            Item Item = null;

            if (!session.GetPlayer().GetInventory().TryGetItem(packet.PopWiredInt(), out Item))
            {
                return;
            }

            string DecorationKey = string.Empty;

            switch (Item.Data.Behaviour)
            {
                case ItemBehaviour.FLOOR:
                    DecorationKey = "floor";
                    break;

                case ItemBehaviour.WALLPAPER:
                    DecorationKey = "wallpaper";
                    break;

                case ItemBehaviour.LANDSCAPE:
                    DecorationKey = "landscape";
                    break;
            }

            if (string.IsNullOrEmpty(DecorationKey))
            {
                return;
            }

            Item RemovedItem = null;

            if (!session.GetPlayer().GetInventory().TryRemoveItem(Item.Id, out RemovedItem))
            {
                return;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("DELETE FROM `items` WHERE `id` = @id;");
                    DbCon.AddParameter("id", RemovedItem.Id);
                    DbCon.ExecuteNonQuery();

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    session.GetPlayer().GetInventory().TryAddItem(RemovedItem);
                    return;
                }
            }

            session.GetPlayer().GetAvatar().GetCurrentRoom().ApplyDecoration(DecorationKey, Item.Flags);

            session.SendPacket(new FurniListRemoveComposer(RemovedItem));
            session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new RoomPropertyComposer(DecorationKey, Item.Flags));
        }
    }
}
