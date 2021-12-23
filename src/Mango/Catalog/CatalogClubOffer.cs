using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Subscriptions;
using Mango.Database.Exceptions;

namespace Mango.Catalog
{
    class CatalogClubOffer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CatalogClubOfferType Type { get; set; }
        public int CreditsCost { get; set; }
        public int LengthDays { get; set; }

        public CatalogClubOffer(int Id, string Name, string Type, int CreditsCost, int LengthDays)
        {
            this.Id = Id;
            this.Name = Name;

            if (Type != "basic" && Type != "vip" && Type != "upgrade")
                throw new DatabaseException(string.Format("Expected data to be 'basic' or 'vip' or 'upgrade' but was '{0}'.", Type));

            switch (Type)
            {
                case "basic":
                    this.Type = CatalogClubOfferType.BASIC;
                    break;

                case "vip":
                    this.Type = CatalogClubOfferType.VIP;
                    break;

                case "upgrade":
                    this.Type = CatalogClubOfferType.VIPUPGRADE;
                    break;
            }

            this.CreditsCost = CreditsCost;
            this.LengthDays = LengthDays;
        }

        public bool IsUpgrade
        {
            get
            {
                return this.Type == CatalogClubOfferType.VIPUPGRADE;
            }
        }

        public double LengthSeconds
        {
            get
            {
                return 86400 * this.LengthDays;
            }
        }

        public int LengthMonths
        {
            get
            {
                int CorrectedLength = this.LengthDays;

                if (IsUpgrade)
                {
                    CorrectedLength += 31;
                }

                return (int)(Math.Ceiling(((double)(CorrectedLength / 31))));
            }
        }

        public int Level
        {
            get
            {
                return (this.Type == CatalogClubOfferType.BASIC ? 1 : 2);
            }
        }
    }
}
