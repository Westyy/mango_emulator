using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Availability
{
    class LoginFailedHotelClosedComposer : ServerPacket
    {
        public LoginFailedHotelClosedComposer(int ReopeningHour, int ReopeningMinute)
            : base(ServerPacketHeader.LoginFailedHotelClosedMessageComposer)
        {
            base.WriteInteger(ReopeningHour);
            base.WriteInteger(ReopeningMinute);
        }
    }
}
