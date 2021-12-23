using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Advertisement
{
    class RoomAdComposer : ServerPacket
    {
        public RoomAdComposer()
            : base(ServerPacketHeadersNew.RoomAdMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
