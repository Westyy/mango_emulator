using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Availability
{
    class InfoHotelClosingComposer : ServerPacket
    {
        public InfoHotelClosingComposer(int ShuttingdownInMinutes)
            : base(ServerPacketHeader.InfoHotelClosingMessageComposer)
        {
            base.WriteInteger(ShuttingdownInMinutes);
        }
    }
}
