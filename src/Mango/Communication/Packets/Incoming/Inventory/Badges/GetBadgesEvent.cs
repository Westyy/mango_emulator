using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Inventory.Badges;
using Mango.Badges;

namespace Mango.Communication.Packets.Incoming.Inventory.Badges
{
    class GetBadgesEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            ICollection<BadgeData> NonEquipped = session.GetPlayer().GetBadges().StaticBadges;
            ICollection<BadgeData> Equipped = session.GetPlayer().GetBadges().EquippedBadges;

            session.SendPacket(new BadgesComposer(NonEquipped, Equipped));
        }
    }
}
