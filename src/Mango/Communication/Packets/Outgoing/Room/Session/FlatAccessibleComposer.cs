using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Session
{
    class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer()
            : base(ServerPacketHeadersNew.FlatAccessibleMessageComposer)
        {
            base.WriteString("");
        }
    }
}
