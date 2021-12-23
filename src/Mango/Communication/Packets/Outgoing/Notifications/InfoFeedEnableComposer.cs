using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class InfoFeedEnableComposer : ServerPacket
    {
        public InfoFeedEnableComposer(bool enabled)
            : base(ServerPacketHeader.InfoFeedEnableMessageComposer)
        {
            base.WriteBoolean(enabled);
        }
    }
}
