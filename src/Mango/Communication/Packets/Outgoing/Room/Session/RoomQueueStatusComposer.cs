using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Session
{
    class RoomQueueStatusComposer : ServerPacket
    {
        public RoomQueueStatusComposer()
            : base(ServerPacketHeadersNew.RoomQueueStatusMessageComposer)
        {
        }
    }
}
