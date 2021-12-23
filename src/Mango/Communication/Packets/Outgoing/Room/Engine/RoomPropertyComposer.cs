using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class RoomPropertyComposer : ServerPacket
    {
        public RoomPropertyComposer(string key, string value)
            : base(ServerPacketHeadersNew.RoomPropertyMessageComposer)
        {
            base.WriteString(key);
            base.WriteString(value);
        }
    }
}
