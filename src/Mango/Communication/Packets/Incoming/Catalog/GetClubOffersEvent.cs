using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Catalog;
using Mango.Communication.Packets.Outgoing.Catalog;
using Mango.Subscriptions;

namespace Mango.Communication.Packets.Incoming.Catalog
{
    class GetClubOffersEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            bool HasActiveSubscription = false;
            Subscription ClubSubscription = null;

            if (session.GetPlayer().GetSubscriptions().TryGetSubscription("habbo_club", out ClubSubscription))
            {
                HasActiveSubscription = true;
            }

            session.SendPacket(new HabboClubOffersComposer(Mango.GetServer().GetCatalogManager().GetClubOffers(),
                ClubSubscription != null && ClubSubscription.IsActive ? ClubSubscription.TimestampExpires : UnixTimestamp.GetNow()));

            /*List<CatalogClubOffer> CorrectedOffers = new List<CatalogClubOffer>();

            foreach (CatalogClubOffer offer in Mango.GetServer().GetCatalogManager().GetClubOffers())
            {
                if (HasActiveSubscription)
                {
                    if ((offer.IsUpgrade && ClubSubscription.CurrentLevel != 1) ||
                        (offer.Level == 1 && ClubSubscription.CurrentLevel == 2) ||
                        (offer.Level == 2 && !offer.IsUpgrade && ClubSubscription.CurrentLevel == 1))
                    {
                        continue;
                    }
                }
                else
                {
                    if (offer.IsUpgrade)
                    {
                        continue;
                    }
                }

                CorrectedOffers.Add(offer);
            }

            session.SendPacket(new HabboClubOffersComposer(CorrectedOffers,
                ClubSubscription != null && ClubSubscription.IsActive ? ClubSubscription.TimestampExpires : UnixTimestamp.GetNow()));*/
        }
    }
}
