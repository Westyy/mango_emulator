using Mango.Catalog;
using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class HabboClubExtendComposer : ServerPacket
    {
        public HabboClubExtendComposer(CatalogClubOffer Offer, double BaseTimestamp)
            : base(ServerPacketHeadersNew.HabboClubExtendComposer)
        {
            DateTime ExpireTime = UnixTimestamp.FromUnixTimestamp(BaseTimestamp + Offer.LengthSeconds);

            base.WriteInteger(Offer.Id);
            base.WriteString(Offer.Name);
            base.WriteInteger(Offer.CreditsCost); // you save
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

            base.WriteInteger(100); // price
            base.WriteInteger(5); // valid for x days
        }
    }
}
