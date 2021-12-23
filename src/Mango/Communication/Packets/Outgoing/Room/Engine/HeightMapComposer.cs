using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer(String map)
            : base(ServerPacketHeadersNew.HeightMapMessageComposer)
        {
            base.WriteString(map);
        }
    }
}
