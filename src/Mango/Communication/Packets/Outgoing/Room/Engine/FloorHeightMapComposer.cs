using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class FloorHeightMapComposer : ServerPacket
    {
        public FloorHeightMapComposer(string map)
            : base(ServerPacketHeadersNew.FloorHeightMapMessageComposer)
        {
            base.WriteString(map);
        }
    }
}
