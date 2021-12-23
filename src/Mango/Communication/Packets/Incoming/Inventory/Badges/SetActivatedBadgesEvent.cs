using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Badges;
using Mango.Communication.Packets.Outgoing.Users;
using MySql.Data.MySqlClient;

namespace Mango.Communication.Packets.Incoming.Inventory.Badges
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int i = 0;

            Dictionary<int, BadgeData> NewOrder = new Dictionary<int, BadgeData>();

            while (Packet.RemainingLength > 0)
            {
                if (i > 5)
                {
                    continue;
                }

                i++;

                int SlotId = Packet.PopWiredInt();
                string BadgeCode = Packet.PopString();

                BadgeData ServerBadgeData = null;

                if (!Mango.GetServer().GetBadgeManager().TryGetBadgeByCode(BadgeCode, out ServerBadgeData))
                {
                    continue;
                }

                if (!Session.GetPlayer().GetBadges().Contains(BadgeCode) || SlotId >= 6 || SlotId <= 0 || NewOrder.ContainsKey(SlotId))
                {
                    continue;
                }

                NewOrder.Add(SlotId, ServerBadgeData);
            }

            Session.GetPlayer().GetBadges().UpdateBadgeOrder(Session.GetPlayer().Id, NewOrder);

            if (Session.GetPlayer().GetAvatar().InRoom)
            {
                Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new HabboUserBadgesComposer(Session.GetPlayer(), NewOrder.Values));
            }
        }
    }
}
