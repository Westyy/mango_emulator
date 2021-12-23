using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;
using log4net;

namespace Mango.Players.Wardrobe
{
    sealed class WardrobeComponent
    {
        private ILog log = LogManager.GetLogger("Mango.Players.Wardrobe.WardrobeComponent");

        /// <summary>
        /// Wardrobe Items with Item ID > WardrobeItem.
        /// </summary>
        private readonly Dictionary<int, WardrobeItem> _wardrobe;

        /// <summary>
        /// When the users figure was last updated.
        /// </summary>
        private double _figureLastUpdated = 0;

        /// <summary>
        /// How long to wait until the figure can be changed again? (should match the clients setting)
        /// </summary>
        private static int _figureWaitTimeInSec = 6;

        public WardrobeComponent()
        {
            this._wardrobe = new Dictionary<int, WardrobeItem>();
        }

        /// <summary>
        /// Initializes the WardrobeComponent.
        /// </summary>
        /// <param name="Player">Player.</param>
        public bool Init(Player Player)
        {
            if (this._wardrobe.Count > 0)
            {
                throw new InvalidOperationException("Wardrobe already initialized.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `wardrobe` WHERE `user_id` = @uid;");
                DbCon.AddParameter("uid", Player.Id);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    try
                    {
                        while (Reader.Read())
                        {
                            this._wardrobe.Add(Reader.GetInt32("slot_id"), new WardrobeItem(Reader.GetInt32("id"), Reader.GetInt32("user_id"),
                                Reader.GetInt32("slot_id"), Reader.GetString("figure"), Reader.GetString("gender")));
                        }
                    }
                    catch (DatabaseException)
                    {
                        log.Error("Failed to load wardrobe item");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the figure can be changed.
        /// </summary>
        public bool CanChangeFigure
        {
            get
            {
                return (this._figureLastUpdated - UnixTimestamp.GetNow()) > 0 ? false : true;
            }
        }

        /// <summary>
        /// Sets that the users figure has been updated.
        /// </summary>
        public void SetFigureUpdated()
        {
            this._figureLastUpdated = UnixTimestamp.GetNow() + _figureWaitTimeInSec;
        }

        /// <summary>
        /// Try to retrieve a WardrobeItem based on the slot id.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public bool TryGet(int Id, out WardrobeItem Item)
        {
            return this._wardrobe.TryGetValue(Id, out Item);
        }

        public bool TryAdd(int Id, WardrobeItem Item)
        {
            if (this._wardrobe.ContainsKey(Id))
            {
                return false;
            }

            this._wardrobe.Add(Id, Item);
            return true;
        }

        public IDictionary<int, WardrobeItem> WardobeItems
        {
            get
            {
                return this._wardrobe;
            }
        }
    }
}
