using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Availability
{
    class InfoHotelClosedComposer : ServerPacket
    {
        public InfoHotelClosedComposer(int ReopeningHour, int ReopeningMinute)
            : base(ServerPacketHeader.InfoHotelClosedMessageComposer)
        {
            base.WriteInteger(ReopeningHour);
            base.WriteInteger(ReopeningMinute);
            base.WriteBoolean(false); // DC All i guess
        }
    }
}
