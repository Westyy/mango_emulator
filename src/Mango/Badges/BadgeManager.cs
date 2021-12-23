using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Badges
{
    sealed class BadgeManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Badges.BadgeManager");

        private readonly Dictionary<int, BadgeData> _badges = new Dictionary<int, BadgeData>();
        private readonly Dictionary<string, int> _badgeCodeIndex = new Dictionary<string, int>();

        public BadgeManager()
        {
        }

        public void Init()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `badge_definitions`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._badges.Add(Reader.GetInt32("id"), new BadgeData(Reader.GetInt32("id"), Reader.GetString("code")));
                            this._badgeCodeIndex.Add(Reader.GetString("code"), Reader.GetInt32("id"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load BadgeDefinition for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this._badges.Count + " badge definitions.");
        }

        public bool TryGetBadge(int Id, out BadgeData Badge)
        {
            return this._badges.TryGetValue(Id, out Badge);
        }

        public bool TryGetBadgeByCode(string Code, out BadgeData Badge)
        {
            int Id = 0;

            if (this._badgeCodeIndex.TryGetValue(Code, out Id))
            {
                return this._badges.TryGetValue(Id, out Badge);
            }
            else
            {
                Badge = null;
                return false;
            }
        }
    }
}
