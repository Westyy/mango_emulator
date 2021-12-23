using System;
using System.Collections.Generic;
using log4net;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Navigator
{
    sealed class NavigatorManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Navigator.NavigatorManager");

        private readonly List<FlatCategory> FlatCategories = new List<FlatCategory>();
        private readonly List<NavigatorOfficial> FeaturedFlats = new List<NavigatorOfficial>();
        private readonly Dictionary<string, int> SearchEventQueries = new Dictionary<string, int>();

        public NavigatorManager() { }

        public void Init()
        {
            InitFlatCategories();
            InitFrontpage();
        }

        private void InitFlatCategories()
        {
            if (this.FlatCategories.Count > 0)
            {
                throw new InvalidOperationException("FlatCategory already has contents, cannot use this method to re-initialize or reload the FlatCategory!");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `flat_categories`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this.FlatCategories.Add(new FlatCategory(Reader.GetInt32("id"), Reader.GetInt32("order_num"),
                                Reader.GetInt32("visible"), Reader.GetString("title"), Reader.GetInt32("allow_trading")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Flat Category for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this.FlatCategories.Count + " flat categories.");
        }

        public void InitFrontpage()
        {
            if (this.FeaturedFlats.Count > 0)
            {
                throw new InvalidOperationException("FeaturedFlats already has contents, cannot use this method to re-initialize or reload the FeaturedFlats!");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `navigator_frontpage`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this.FeaturedFlats.Add(new NavigatorOfficial(Reader.GetInt32("id"), Reader.GetInt32("enabled"),
                                Reader.GetInt32("parent_id"), Reader.GetInt32("room_id"), Reader.GetInt32("is_category"),
                                Reader.GetString("display_type"), Reader.GetString("image_type"), Reader.GetString("name"),
                                Reader.GetString("descr"), Reader.GetString("image_src"), Reader.GetString("banner_label"),
                                Reader.GetInt32("category_autoexpand")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Navigator Frontpage for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this.FeaturedFlats.Count + " navigator frontpage items.");
        }

        public List<FlatCategory> GetFlatCategories()
        {
            return this.FlatCategories;
        }

        public List<NavigatorOfficial> GetFeaturedFlats()
        {
            return this.FeaturedFlats;
        }

        public Dictionary<string, int> GetSearchEventQueries()
        {
            return this.SearchEventQueries;
        }
    }
}
