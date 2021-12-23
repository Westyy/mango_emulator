using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer()
            : base(ServerPacketHeadersNew.FlatAccessDeniedMessageComposer)
        {
        }
    }
}
