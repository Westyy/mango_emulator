using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Subscriptions;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer(Subscription Subscription, bool BoughtFromCatalog = false)
            : base(ServerPacketHeadersNew.ScrSendUserInfoComposer)
        {
            int DisplayMonths = 0;
            int DisplayDays = 0;

            if (Subscription != null)
            {
                TimeSpan TimeSpan = UnixTimestamp.FromUnixTimestamp(Subscription.TimestampExpires) - DateTime.Now;

                if (TimeSpan.TotalSeconds > 0)
                {
                    DisplayMonths = (int)Math.Floor((double)(TimeSpan.Days / 31.0));
                    DisplayDays = TimeSpan.Days - (31 * DisplayMonths);
                }
            }

            base.WriteString("habbo_club");
            base.WriteInteger(DisplayDays);
            base.WriteInteger(2);
            base.WriteInteger(DisplayMonths);
            base.WriteInteger(1);
            base.WriteBoolean(Subscription == null ? false : true); // hc
            base.WriteBoolean(Subscription == null ? false : true); // vip
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(Subscription == null ? 0 : 495);

           /* base.WriteString("habbo_club");
            base.WriteInteger(DisplayDays);
            base.WriteInteger(2);
            //base.WriteBoolean(Subscription.IsActive);
            base.WriteInteger(DisplayMonths);
            base.WriteInteger(BoughtFromCatalog ? 2 : 1);
            base.WriteInteger(1);
            base.WriteBoolean(Subscription.IsActive && Subscription.CurrentLevel == 2);
            base.WriteInteger(0); // past hc days
            base.WriteInteger(0); // past vip days
            base.WriteInteger(0); // discount msg
            base.WriteInteger(30); // regular price
            base.WriteInteger(25); // your price*/
        }
    }
}
