using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Catalog;
using Mango.Subscriptions;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class HabboClubOffersComposer : ServerPacket
    {
        public HabboClubOffersComposer(ICollection<CatalogClubOffer> Offers, double BaseTimestamp)
            : base(ServerPacketHeadersNew.HabboClubOffersMessageComposer)
        {
            base.WriteInteger(Offers.Count);

            foreach (CatalogClubOffer Offer in Offers)
            {
                DateTime ExpireTime = UnixTimestamp.FromUnixTimestamp(BaseTimestamp + Offer.LengthSeconds);

                base.WriteInteger(Offer.Id);
                base.WriteString(Offer.Name);
                base.WriteInteger(Offer.CreditsCost);
                //base.WriteBoolean(Offer.IsUpgrade);
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteBoolean(Offer.Level == 2);
                base.WriteInteger(Offer.LengthMonths);
                base.WriteInteger(Offer.LengthDays);
                base.WriteInteger(0);
                base.WriteInteger(ExpireTime.Year);
                base.WriteInteger(ExpireTime.Day);
                base.WriteInteger(ExpireTime.Month);
            }

            base.WriteInteger(1);
        }
    }
}
