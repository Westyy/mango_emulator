using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class VoucherRedeemErrorComposer : ServerPacket
    {
        public VoucherRedeemErrorComposer(int Code)
            : base(ServerPacketHeadersNew.VoucherRedeemErrorMessageComposer)
        {
            base.WriteString(Code.ToString());
        }
    }
}
