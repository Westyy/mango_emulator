using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class PurchaseErrorComposer : ServerPacket
    {
        public PurchaseErrorComposer()
            : base(ServerPacketHeadersNew.PurchaseErrorMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
