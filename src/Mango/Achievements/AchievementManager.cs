using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Achievements
{
    sealed class AchievementManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Achievements.AchievementManager");

        private readonly Dictionary<string, AchievementData> _achievements = new Dictionary<string, AchievementData>();

        public AchievementManager()
        {
        }

        public void Init()
        {
            if (this._achievements.Count > 0)
            {
                throw new InvalidOperationException("Achievements cannot be re-initialized from this method.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `achievements`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            string Group = Reader.GetString("group_name");

                            if (!this._achievements.ContainsKey(Group))
                            {
                                this._achievements.Add(Group, new AchievementData(Reader.GetInt32("id"), Group, Reader.GetString("category")));
                            }

                            this._achievements[Group].StoreLevel(new AchievementLevel(Reader.GetInt32("level"), Reader.GetInt32("reward_pixels"),
                                Reader.GetInt32("reward_points"), Reader.GetInt32("progress_needed")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load BadgeDefinition for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this._achievements.Count + " achievements.");
        }

        public ICollection<AchievementData> AchievementList
        {
            get
            {
                return this._achievements.Values;
            }
        }

        public ICollection<AchievementData> GamesAchievements
        {
            get
            {
                return new List<AchievementData>();
            }
        }
    }
}
