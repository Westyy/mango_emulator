using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Items;

namespace Mango.Catalog
{
    class CatalogItem
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int BaseId { get; set; }
        public ItemData Data { get; set; }
        public bool Enabled { get; set; }
        public string DisplayName { get; set; }
        public int CostCredits { get; set; }
        public int CostPixels { get; set; }
        public int Amount { get; set; }
        public string PresetFlags { get; set; }
        public int ClubRestriction { get; set; }

        public CatalogItem(int Id, int PageId, int BaseId, ItemData Data, string Enabled, string DisplayName, int Credits, int Pixels,
            int Amount, string PresetFlags, int ClubRestriction)
        {
            this.Id = Id;
            this.PageId = PageId;
            this.Data = Data;
            this.Enabled = Enabled.ToLower() == "y" ? true : false;
            this.DisplayName = DisplayName;
            this.CostCredits = Credits;
            this.CostPixels = Pixels;
            this.Amount = Amount;
            this.PresetFlags = PresetFlags;
            this.ClubRestriction = ClubRestriction;
        }
    }
}
