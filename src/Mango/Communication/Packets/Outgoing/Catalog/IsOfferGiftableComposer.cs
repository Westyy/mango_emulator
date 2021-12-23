using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class IsOfferGiftableComposer : ServerPacket
    {
        public IsOfferGiftableComposer(int ItemId, bool CanGift)
            : base(ServerPacketHeader.IsOfferGiftableMessageComposer)
        {
            base.WriteInteger(ItemId);
            base.WriteBoolean(CanGift); // needs checking
        }
    }
}
