using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Mango.Players.Achievements;
using Mango.Badges;
using Mango.Achievements;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;
using log4net;

namespace Mango.Players.Badges
{
    sealed class BadgeComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Players.Badges.BadgeComponent");

        private ConcurrentDictionary<int, BadgeData> _equippedBadges = new ConcurrentDictionary<int, BadgeData>();
        private ConcurrentDictionary<int, BadgeData> _staticBadges = new ConcurrentDictionary<int, BadgeData>();
        private ConcurrentDictionary<string, BadgeData> _achievementBadges = new ConcurrentDictionary<string, BadgeData>();
        private ConcurrentDictionary<string, BadgeData> _badgeIndex = new ConcurrentDictionary<string, BadgeData>();

        public BadgeComponent()
        {
        }

        public bool Init(Player Player, AchievementComponent Achievements)
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `badges` WHERE `user_id` = @id;");
                DbCon.AddParameter("id", Player.Id);

                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int Id = Reader.GetInt32("id");
                            int BadgeId = Reader.GetInt32("badge_id");
                            string SourceType = Reader.GetString("source_type");
                            string SourceData = Reader.GetString("source_data");
                            int SlotId = Reader.GetInt32("slot_id");

                            BadgeData Badge = null;

                            if (!Mango.GetServer().GetBadgeManager().TryGetBadge(BadgeId, out Badge))
                            {
                                using (var DbCon2 = Mango.GetServer().GetDatabase().GetConnection())
                                {
                                    try
                                    {
                                        DbCon2.Open();
                                        DbCon2.BeginTransaction();

                                        DbCon2.SetQuery("DELETE FROM `badges` WHERE id = @id;");
                                        DbCon2.AddParameter("id", Id);
                                        DbCon2.ExecuteNonQuery();

                                        DbCon.Commit();
                                    }
                                    catch (MySqlException) { DbCon.Rollback(); }
                                }

                                continue;
                            }

                            BadgeData BadgeToEquip = null;

                            if (SourceType == "static")
                            {
                                BadgeToEquip = Badge;
                                this._staticBadges.TryAdd(Badge.Id, Badge);
                            }
                            else if (SourceType == "achievement")
                            {
                                if (this._achievementBadges.ContainsKey(SourceData))
                                {
                                    continue;
                                }

                                Achievement Achievement = null;

                                if (!Achievements.TryGetAchievementData(SourceData, out Achievement) || Achievement.Level < 1)
                                {
                                    using (var DbCon2 = Mango.GetServer().GetDatabase().GetConnection())
                                    {
                                        try
                                        {
                                            DbCon2.Open();
                                            DbCon2.BeginTransaction();

                                            DbCon2.SetQuery("DELETE FROM `badges` WHERE id = @id;");
                                            DbCon2.AddParameter("id", Id);
                                            DbCon2.ExecuteNonQuery();

                                            DbCon2.Commit();
                                        }
                                        catch (MySqlException) { DbCon2.Rollback(); }
                                    }

                                    continue;
                                }

                                string Code = Achievement.BadgeCodeForLevel;

                                if (Badge.Code == Code)
                                {
                                    BadgeToEquip = Badge;
                                }
                                else
                                {
                                    BadgeData BadgeRef = null;

                                    if (Mango.GetServer().GetBadgeManager().TryGetBadgeByCode(Code, out BadgeRef))
                                    {
                                        BadgeToEquip = BadgeRef;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                this._achievementBadges.TryAdd(SourceData, BadgeToEquip);
                            }

                            if (BadgeToEquip != null)
                            {
                                if (!this._equippedBadges.ContainsKey(SlotId) && SlotId >= 1 && SlotId <= 5)
                                {
                                    this._equippedBadges.TryAdd(SlotId, BadgeToEquip);
                                }

                                this._badgeIndex.TryAdd(BadgeToEquip.Code, BadgeToEquip);
                            }
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load User Badge for ID [" + Reader.GetInt32("id") + "]", ex);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void UpdateBadgeOrder(int UserId, Dictionary<int, BadgeData> NewOrder)
        {
            var Clone = new ConcurrentDictionary<int, BadgeData>(this._equippedBadges);

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    this._equippedBadges.Clear();

                    DbCon.SetQuery("UPDATE `badges` SET `slot_id` = 0 WHERE `user_id` = @id");
                    DbCon.AddParameter("id", UserId);
                    DbCon.ExecuteNonQuery();

                    foreach (KeyValuePair<int, BadgeData> kvp in NewOrder)
                    {
                        if (this._equippedBadges.TryAdd(kvp.Key, kvp.Value))
                        {
                            DbCon.SetQuery("UPDATE `badges` SET `slot_id` = @slotid WHERE `user_id` = @uid AND `badge_id` = @bid LIMIT 1;");
                            DbCon.AddParameter("slotid", kvp.Key);
                            DbCon.AddParameter("uid", UserId);
                            DbCon.AddParameter("bid", kvp.Value.Id);
                            DbCon.ExecuteNonQuery();
                        }
                    }

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    this._equippedBadges = Clone;
                }
            }
        }

        public bool Contains(string BadgeCode)
        {
            return this._badgeIndex.ContainsKey(BadgeCode);
        }

        public ICollection<BadgeData> EquippedBadges
        {
            get
            {
                return this._equippedBadges.Values;
            }
        }

        public ICollection<BadgeData> StaticBadges
        {
            get
            {
                return this._staticBadges.Values;
            }
        }
    }
}
