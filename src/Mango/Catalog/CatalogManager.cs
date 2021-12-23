using System;
using System.Collections.Generic;
using log4net;
using Mango.Items;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Catalog
{
    sealed class CatalogManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Catalog.CatalogManager");

        private readonly Dictionary<int, CatalogPage> _pages;

        private readonly Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private readonly Dictionary<int, CatalogItem> _itemsIdIndex;
        private readonly Dictionary<string, CatalogItem> _itemsNameIndex;

        private readonly Dictionary<int, CatalogClubOffer> _clubOffers;

        public CatalogManager()
        {
            this._pages = new Dictionary<int, CatalogPage>();
            this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            this._itemsIdIndex = new Dictionary<int, CatalogItem>();
            this._itemsNameIndex = new Dictionary<string, CatalogItem>();
            this._clubOffers = new Dictionary<int, CatalogClubOffer>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (ItemDataManager == null)
            {
                throw new InvalidOperationException("ItemDataManager cannot be null, Catalog items need to load data from the ItemDataManager!");
            }

            if (_pages.Count > 0 || _items.Count > 0 || _itemsIdIndex.Count > 0 || _itemsNameIndex.Count > 0 || _clubOffers.Count > 0)
            {
                _pages.Clear();
                _items.Clear();
                _itemsIdIndex.Clear();
                _itemsNameIndex.Clear();
                _clubOffers.Clear();
                //throw new InvalidOperationException("Catalog has already been initialized. It cannot be re-initialized!");
            }

            //this.Pages.Add (-1, new CatalogPage(-1, 0, string.Empty, 0, 0, 0, false, false, string.Empty, null, null, new List<CatalogItem>()));

            int ItemsLoaded = 0;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `catalog_items` WHERE `enabled` = 'Y' ORDER BY `name` DESC;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int PageId = Reader.GetInt32("page_id");
                            int BaseId = Reader.GetInt32("base_id");

                            if (!this._items.ContainsKey(PageId))
                            {
                                this._items[PageId] = new Dictionary<int, CatalogItem>();
                            }

                            ItemData Data = null;

                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Failed to load Catalog item with id: " + Reader.GetInt32("id") + " and name: " + Reader.GetString("name") + " this item will be skipped.");
                                continue;
                            }

                            CatalogItem CatalogItem = new CatalogItem(Reader.GetInt32("id"), Reader.GetInt32("page_id"),
                                Reader.GetInt32("base_id"), Data, Reader.GetString("enabled"), Reader.GetString("name"),
                                Reader.GetInt32("cost_credits"), Reader.GetInt32("cost_pixels"), Reader.GetInt32("amount"),
                                Reader.GetString("preset_flags"), Reader.GetInt32("club_restriction"));

                            this._items[CatalogItem.PageId].Add(CatalogItem.Id, CatalogItem);
                            this._itemsIdIndex[CatalogItem.Id] = CatalogItem;
                            this._itemsNameIndex[CatalogItem.DisplayName] = CatalogItem;

                            ItemsLoaded++;
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load CatalogItem for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            int PagesLoaded = 0;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `catalog_pages` WHERE `enabled` = 'Y' ORDER BY `order_num` ASC;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._pages.Add(Reader.GetInt32("id"), new CatalogPage(Reader.GetInt32("id"), Reader.GetInt32("parent_id"),
                                Reader.GetInt32("order_num"), Reader.GetString("enabled"), Reader.GetString("title"), Reader.GetInt32("icon"),
                                Reader.GetInt32("color"), Reader.GetString("required_right"), Reader.GetString("visible"), Reader.GetString("dummy_page"),
                                Reader.GetString("template"), Reader.GetString("page_strings_1"), Reader.GetString("page_strings_2"),
                                this._items.ContainsKey(Reader.GetInt32("id")) ? this._items[Reader.GetInt32("id")] : new Dictionary<int, CatalogItem>()));

                            PagesLoaded++;
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load CatalogPage for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }

                log.Info("Loaded " + PagesLoaded + " catalog pages & " + ItemsLoaded + " catalog items.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `catalog_subscriptions`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._clubOffers.Add(Reader.GetInt32("id"), new CatalogClubOffer(Reader.GetInt32("id"), Reader.GetString("name"),
                                Reader.GetString("type"), Reader.GetInt32("cost_credits"), Reader.GetInt32("length_days")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load CatalogClubSubscription for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this._clubOffers.Count + " catalog subscriptions.");
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return this._pages.TryGetValue(pageId, out page);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return this._pages.Values;
        }

        public ICollection<CatalogClubOffer> GetClubOffers()
        {
            return this._clubOffers.Values;
        }

        public CatalogClubOffer FirstOffer()
        {
            return this._clubOffers[1];
        }

        public bool TryGetClubOffer(int ItemId, out CatalogClubOffer Offer)
        {
            return this._clubOffers.TryGetValue(ItemId, out Offer);
        }
    }
}
