using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class HabboGroupBadgesComposer : ServerPacket
    {
        public HabboGroupBadgesComposer()
            : base(ServerPacketHeadersNew.HabboGroupBadgesMessageComposer)
        {
            // count
            // foreach => group id
            //         => string/wb badge code
            //base.WriteString("Ib[ZCs58116s04078s04072s52074889902cf4440630470f222ad5c6489d7");
        }
    }
}
