using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class MOTDNotificationComposer : ServerPacket
    {
        public MOTDNotificationComposer(string Message)
            : base(ServerPacketHeadersNew.MOTDNotificationComposer)
        {
            base.WriteInteger(1);
            base.WriteString(Message);
        }
    }
}
