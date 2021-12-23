using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class HeightMapUpdateComposer : ServerPacket
    {
        public HeightMapUpdateComposer(Heightmap map)
            : base(ServerPacketHeadersNew.HeightMapUpdateMessageComposer)
        {
            base.WriteString(map.ToString());
        }
    }
}
