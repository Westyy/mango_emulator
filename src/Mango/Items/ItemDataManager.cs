using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;
using Mango.Utilities;
using log4net;
using Mango.Database.Exceptions;
using System.Threading;
using Mango.Attributes;

namespace Mango.Items
{
    sealed class ItemDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Items.ItemDataManager");

        /// <summary>
        /// Stores the ItemData.
        /// </summary>
        private readonly Dictionary<int, ItemData> _items;

        public ItemDataManager()
        {
            this._items = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (_items.Count > 0)
            {
                this._items.Clear();
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `item_definitions`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._items.Add(Reader.GetInt32("id"), new ItemData(Reader.GetInt32("id"), Reader.GetInt32("sprite_id"),
                                Reader.GetString("name"), Reader.GetString("type"), Reader.GetString("behavior"), Reader.GetString("stacking_behavior"),
                                Reader.GetString("walkable"), Reader.GetInt32("behavior_data"), Reader.GetInt32("room_limit"),
                                Reader.GetInt32("size_x"), Reader.GetInt32("size_y"), Reader.GetFloat("height"), Reader.GetInt32("allow_recycling"),
                                Reader.GetInt32("allow_trading"), Reader.GetInt32("allow_selling"), Reader.GetInt32("allow_gifting"), Reader.GetInt32("allow_inventory_stacking")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Item for Item ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this._items.Count + " item definitions.");
        }

        public bool GetItem(int Id, out ItemData Item)
        {
            if (this._items.TryGetValue(Id, out Item))
            {
                return true;
            }

            return false;
        }

        public bool Contains(int Id)
        {
            return this._items.ContainsKey(Id);
        }

        /// <summary>
        /// Clears Item Data.
        /// </summary>
        [DevNotice("Only used for debugging.")]
        public void Clear()
        {
            this._items.Clear();
        }
    }
}
