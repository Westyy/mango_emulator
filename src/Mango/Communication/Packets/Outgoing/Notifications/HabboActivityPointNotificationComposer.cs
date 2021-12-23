using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int balance, int notifyAmount)
            : base(ServerPacketHeadersNew.HabboActivityPointNotificationMessageComposer)
        {
            base.WriteInteger(balance);
            base.WriteInteger(notifyAmount);
            base.WriteInteger(0);
        }
    }
}
