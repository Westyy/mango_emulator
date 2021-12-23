using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class HabboBroadcastComposer : ServerPacket
    {
        public HabboBroadcastComposer(string Text)
            : base(ServerPacketHeadersNew.HabboBroadcastMessageComposer)
        {
            base.WriteString(Text);
        }
    }
}
