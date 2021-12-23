using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Catalog;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage page)
            : base(ServerPacketHeadersNew.CatalogPageMessageComposer)
        {
            base.WriteInteger(page.Id);
            base.WriteString(page.Template);
            base.WriteInteger(page.PageStrings1.Count);

            foreach (string s in page.PageStrings1)
            {
                base.WriteString(s);
            }

            base.WriteInteger(page.PageStrings2.Count);

            foreach (string s in page.PageStrings2)
            {
                base.WriteString(s);
            }

            base.WriteInteger(page.Items.Count);

            foreach (CatalogItem item in page.Items.Values)
            {
                base.WriteInteger(item.Id);
                base.WriteString(item.DisplayName);
                base.WriteInteger(item.CostCredits);
                base.WriteInteger(item.CostPixels);
                base.WriteInteger(0); // other cost (snow, shells etc)
                base.WriteBoolean(true); // giftable
                base.WriteInteger(1); // items count
                {
                    base.WriteString(ItemTypeUtility.GetTypeLetter(item.Data.Type));
                    base.WriteInteger(item.Data.SpriteId);
                    base.WriteString(item.PresetFlags);
                    base.WriteInteger(item.Amount);
                    //base.WriteInteger(-1);
                    base.WriteBoolean(false); // limited
                    {
                        //base.WriteInteger(5); // Current sells
                        //base.WriteInteger(3); // Left
                    }
                }
                base.WriteInteger(item.ClubRestriction); // new
                base.WriteBoolean(this.CanSelectAmount(item));
            }

            base.WriteInteger(-1);
            base.WriteBoolean(false);
        }

        private bool CanSelectAmount(CatalogItem Item)
        {
            if (Item.DisplayName.StartsWith("cf", StringComparison.CurrentCultureIgnoreCase)
                || Item.DisplayName.StartsWith("cfc", StringComparison.CurrentCultureIgnoreCase)
                || Item.Amount > 1 || Item.Data.Behaviour == ItemBehaviour.EXCHANGE_ITEM)
            {
                return false;
            }

            return true;
        }
    }
}
