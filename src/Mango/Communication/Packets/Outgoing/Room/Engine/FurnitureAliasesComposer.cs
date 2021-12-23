using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class FurnitureAliasesComposer : ServerPacket
    {
        public FurnitureAliasesComposer()
            : base(ServerPacketHeadersNew.FurnitureAliasesMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
