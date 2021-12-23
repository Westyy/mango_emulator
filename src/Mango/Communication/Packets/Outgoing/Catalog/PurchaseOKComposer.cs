using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Catalog;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class PurchaseOKComposer : ServerPacket
    {
        public PurchaseOKComposer(CatalogItem item)
            : base(ServerPacketHeadersNew.PurchaseOKMessageComposer)
        {
            base.WriteInteger(item.Data.Id);
            base.WriteString(item.DisplayName);
            base.WriteInteger(item.CostCredits);
            base.WriteInteger(item.CostPixels);
            base.WriteInteger(0);
            base.WriteBoolean(true);
            base.WriteInteger(1);
            base.WriteString(ItemTypeUtility.GetTypeLetter(item.Data.Type));
            base.WriteInteger(item.Data.SpriteId);
            base.WriteString(item.PresetFlags);
            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteString("");
            base.WriteInteger(1);
        }

        public PurchaseOKComposer(CatalogClubOffer Offer)
            : base(ServerPacketHeadersNew.PurchaseOKMessageComposer)
        {
            base.WriteInteger(Offer.Id);
            base.WriteString(Offer.Name);
            base.WriteInteger(Offer.CreditsCost);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteBoolean(false);
        }
    }
}
