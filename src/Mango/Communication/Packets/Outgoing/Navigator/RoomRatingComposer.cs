using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int ratingData)
            : base(ServerPacketHeadersNew.RoomRatingComposer)
        {
            base.WriteInteger(ratingData);
            base.WriteBoolean(false); //  Room owner? Has voted?
        }
    }
}
