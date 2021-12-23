using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class CanCreateRoomEventComposer : ServerPacket
    {
        public CanCreateRoomEventComposer(int errorCode)
            : base(ServerPacketHeader.CanCreateRoomEventComposer)
        {
            // todo: redo room events
            base.WriteBoolean(errorCode < 1);
            base.WriteInteger(errorCode);
        }
    }
}
