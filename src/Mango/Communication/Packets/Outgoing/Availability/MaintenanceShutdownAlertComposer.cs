using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Availability
{
    class MaintenanceShutdownAlertComposer : ServerPacket
    {
        public MaintenanceShutdownAlertComposer(int Mins)
            : base(ServerPacketHeadersNew.MaintenanceShutdownAlert)
        {
            base.WriteInteger(Mins);
        }
    }
}
