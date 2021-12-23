using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Catalog;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Items;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Inventory.Purse;
using Mango.Communication.Packets.Outgoing.Notifications;
using log4net;
using Mango.Communication.Packets.Outgoing.Catalog;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;
using Mango.Subscriptions;
using Mango.Communication.Packets.Outgoing.Users;
using Mango.Communication.Packets.Outgoing.Handshake;
using Mango.Players.Effects;
using Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects;

namespace Mango.Communication.Packets.Incoming.Catalog
{
    class PurchaseFromCatalogEvent : IPacketEvent
    {
        private static ILog log = LogManager.GetLogger("Mango.Communication.Packets.Incoming.Catalog.PurchaseFromCatalogEvent");

        public void parse(Session session, ClientPacket packet)
        {
            int PageId = packet.PopWiredInt();
            int ItemId = packet.PopWiredInt();
            string Data = packet.PopString();
            int Amount = packet.PopInt();

            CatalogPage Page = null;

            if (!Mango.GetServer().GetCatalogManager().TryGetPage(PageId, out Page))
            {
                return;
            }

            if (Page.DummyPage || !Page.Visible || (Page.RequiredRight.Length > 0 && !session.GetPlayer().GetPermissions().HasRight(Page.RequiredRight)))
            {
                return;
            }

            if (Page.Caption.EndsWith("(Coming Soon)", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            switch (Page.Template)
            {
                default:
                    CatalogItem Item = null;

                    if (!Page.Items.TryGetValue(ItemId, out Item))
                    {
                        return;
                    }

                    if (Item.ClubRestriction > 0)
                    {
                        if (Item.ClubRestriction == 1 && !session.GetPlayer().GetPermissions().HasRight("club_regular"))
                        {
                            return;
                        }

                        if (Item.ClubRestriction == 2 && !session.GetPlayer().GetPermissions().HasRight("club_vip"))
                        {
                            return;
                        }
                    }

                    HandlePurchase(session, Item, Data, Amount);

                    break;

                case "vip_buy":
                    CatalogClubOffer Offer = null;

                    if (!Mango.GetServer().GetCatalogManager().TryGetClubOffer(ItemId, out Offer))
                    {
                        SendErrorMessage(session, "Club offer item does not exist.");
                        return;
                    }

                    if (Offer.CreditsCost > 0 && session.GetPlayer().Credits < Offer.CreditsCost)
                    {
                        SendErrorMessage(session, "Subscription purchase failed.");
                        return;
                    }

                    Subscription ActiveSub = null;

                    if (session.GetPlayer().GetSubscriptions().TryGetSubscription("habbo_club", out ActiveSub))
                    {
                        if (Offer.Level < ActiveSub.CurrentLevel)
                        {
                            SendErrorMessage(session, "Purchase error, Club offer level is lower than active subscription level.");
                            return;
                        }
                    }

                    Subscription SubCreated = null;

                    if (!SubscriptionFactory.AddOrExtend(session.GetPlayer(), "habbo_club", Offer.Level, Offer.LengthSeconds, out SubCreated))
                    {
                        SendErrorMessage(session, "Subscription factory failed.");
                        return;
                    }

                    if (ActiveSub == null)
                    {
                        ActiveSub = SubCreated;
                    }

                    if (Offer.CreditsCost > 0)
                    {
                        session.GetPlayer().UpdateCreditBalance(-Offer.CreditsCost);
                    }

                    int Progress = (int)Math.Ceiling((double)(Offer.LengthDays / 31));

                    if (Progress <= 0)
                    {
                        Progress = 1;
                    }

                    session.GetPlayer().GetPermissions().Reload(session.GetPlayer());

                    session.SendPacket(new CreditBalanceComposer(session.GetPlayer().Credits));
                    session.SendPacket(new PurchaseOKComposer(Offer));
                    session.SendPacket(new ScrSendUserInfoComposer(ActiveSub, true));
                    session.SendPacket(new UserRightsComposer(Offer.Level == 1, Offer.Level == 2, session.GetPlayer().GetPermissions().HasRight("super_admin")));

                    new GetClubOffersEvent().parse(session, null);
                    break;
            }
        }

        private void SendFeatureNotSupported(Session session)
        {
            session.SendPacket(new ModMessageComposer("This feature is not yet supported, we're sorry. (really, we truly are)"));
        }

        private void SendErrorMessage(Session Session, string Error)
        {
            Session.SendPacket(new ModMessageComposer("Catalog error: " + Error));
        }

        private void HandlePurchase(Session Session, CatalogItem Item, string Flags, int Amount)
        {
            /*if (!item.Page.DummyPage || !item.Page.Visible) // to-do: check rights
            {
                return;
            }*/

            //string Colour = "ffffff";
            int TotalCreditCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;

            if (Session.GetPlayer().Credits < TotalCreditCost || Session.GetPlayer().Pixels < TotalPixelCost)
            {
                log.Warn("Player with id: " + Session.GetPlayer().Id + " attempted to purchase item with id " + Item.Id + " with the incorrect amount of pixels or credits.");
                return; // not enough credits or pixels for this item, user most likely tried to packet edit to get here in the first place :-D
            }

            if (Item.Data.Type == ItemType.PET)
            {
                SendFeatureNotSupported(Session);
                return;
            }

            int AmountPurchase = Item.Amount == 1 ? Amount : Item.Amount;

            // do some basic noob proof validation
            if (AmountPurchase > 100)
            {
                AmountPurchase = 100;
            }

            //string[] PetData = null;

            if (Item.PresetFlags.Length > 0)
            {
                Flags = Item.PresetFlags;
            }
            else
            {
                switch (Item.Data.Behaviour)
                {
                    case ItemBehaviour.PET:
                        SendFeatureNotSupported(Session);
                        return;

                    case ItemBehaviour.PRIZE_TROPHY:
                        if (Flags.Length > 255)
                        {
                            Flags = Flags.Substring(0, 255);
                        }

                        Flags = Session.GetPlayer().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" +
                                DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) +
                                StringCharFilter.Escape(Flags.Trim(), true);
                        break;

                    default:
                        Flags = string.Empty;
                        break;
                }
            }

            if (TotalCreditCost > 0)
            {
                Session.GetPlayer().UpdateCreditBalance(-TotalCreditCost);
                Session.SendPacket(new CreditBalanceComposer(Session.GetPlayer().Credits));
            }

            if (TotalPixelCost > 0)
            {
                Session.GetPlayer().UpdatePixelBalance(-TotalPixelCost);
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetPlayer().Pixels, -TotalPixelCost));
            }

            Dictionary<int, List<int>> NewItems = new Dictionary<int, List<int>>();

            //for (int i = 0; i < AmountPurchase; i++)
            {
                switch (Item.Data.Type)
                {
                    default:
                        List<Item> GeneratedGenericItems = new List<Item>();
                        double ExpireTimestamp = 0;

                        if (Item.Data.Behaviour == ItemBehaviour.RENTAL)
                        {
                            ExpireTimestamp = UnixTimestamp.GetNow() + 3600;
                        }

                        switch (Item.Data.Behaviour)
                        {
                            default:
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetPlayer(), Flags, Flags, ExpireTimestamp, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                    }
                                }
                                else
                                {
                                    Item PItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetPlayer(), Flags, Flags, ExpireTimestamp);

                                    if (PItem != null)
                                    {
                                        GeneratedGenericItems.Add(PItem);
                                    }
                                }
                                break;

                            case ItemBehaviour.TELEPORTER:
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetPlayer(), ExpireTimestamp);

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                                break;
                        }

                        foreach (Item PurchasedItem in GeneratedGenericItems)
                        {
                            Session.GetPlayer().GetInventory().TryAddItem(PurchasedItem);
                        }
                        break;

                    case ItemType.EFFECT:
                        AvatarEffect Effect = null;

                        if (Session.GetPlayer().Effects().HasEffect(Item.Data.SpriteId))
                        {
                            Effect = Session.GetPlayer().Effects().GetEffectNullable(Item.Data.SpriteId);

                            if (Effect != null)
                            {
                                Effect.AddToQuantity();
                            }
                        }
                        else
                        {
                            Effect = AvatarEffectFactory.CreateNullable(Session.GetPlayer(), Item.Data.SpriteId, 3600);
                        }

                        if (Effect != null && Session.GetPlayer().Effects().TryAdd(Effect))
                        {
                            Session.SendPacket(new AvatarEffectAddedComposer(Effect));
                        }

                        break;

                    case ItemType.PET:
                        break;
                }
            }

            Session.SendPacket(new PurchaseOKComposer(Item));
            Session.SendPacket(new FurniListUpdateComposer());
        }
    }
}
