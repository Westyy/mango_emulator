using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int RoomId, string Name)
            : base(ServerPacketHeadersNew.FlatCreatedComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteString(Name);
        }
    }
}
