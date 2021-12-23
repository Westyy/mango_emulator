using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Badges;

namespace Mango.Communication.Packets.Outgoing.Inventory.Badges
{
    class BadgesComposer : ServerPacket
    {
        public BadgesComposer(ICollection<BadgeData> nonEquipped, ICollection<BadgeData> equipped)
            : base(ServerPacketHeadersNew.BadgesComposer)
        {
            base.WriteInteger(nonEquipped.Count);

            foreach (BadgeData badge in nonEquipped)
            {
                base.WriteInteger(badge.Id);
                base.WriteString(badge.Code);
            }

            base.WriteInteger(equipped.Count);

            foreach (BadgeData badge in equipped)
            {
                base.WriteInteger(badge.Id);
                base.WriteString(badge.Code);
            }
        }
    }
}
