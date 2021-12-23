using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Inventory.Purse
{
    class CreditBalanceComposer : ServerPacket
    {
        public CreditBalanceComposer(int balance)
            : base(ServerPacketHeadersNew.CreditBalanceComposer)
        {
            base.WriteString(balance.ToString() + ".0");
        }
    }
}
