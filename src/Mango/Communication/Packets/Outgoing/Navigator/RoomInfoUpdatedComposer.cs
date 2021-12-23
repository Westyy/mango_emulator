using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class RoomInfoUpdatedComposer : ServerPacket
    {
        public RoomInfoUpdatedComposer(RoomData data)
            : base(ServerPacketHeadersNew.RoomInfoUpdatedComposer)
        {
            base.WriteInteger(data.Id);
        }
    }
}
