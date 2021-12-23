using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class CanCreateRoomComposer : ServerPacket
    {
        public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
            : base(ServerPacketHeadersNew.CanCreateRoomComposer)
        {
            base.WriteInteger(Error ? 1 : 0);
            base.WriteInteger(MaxRoomsPerUser);
        }
    }
}
