using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Badges;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer(Player player, ICollection<BadgeData> badges)
            : base(ServerPacketHeadersNew.HabboUserBadgesMessageComposer)
        {
            base.WriteInteger(player.Id);
            base.WriteInteger(badges.Count);

            foreach (BadgeData badge in badges)
            {
                base.WriteInteger(badge.Id);
                base.WriteString(badge.Code);
            }
        }
    }
}
