using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Session
{
    class RoomForwardComposer : ServerPacket
    {
        public RoomForwardComposer(RoomData Data)
            : base(ServerPacketHeadersNew.RoomForwardMessageComposer)
        {
            base.WriteBoolean(Data.Type == RoomType.PUBLIC);
            base.WriteInteger(Data.Id);
        }
    }
}
