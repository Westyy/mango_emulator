using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Mango.Achievements;
using MySql.Data.MySqlClient;

namespace Mango.Players.Achievements
{
    sealed class AchievementComponent
    {
        private Player _player = null;

        private readonly ConcurrentDictionary<string, Achievement> _achievements = new ConcurrentDictionary<string, Achievement>();

        public AchievementComponent()
        {
        }

        public bool Init(Player Player)
        {
            if (_achievements.Count > 0)
            {
                throw new InvalidOperationException("Cannot re-initialize achievements from this method.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.SetQuery("SELECT * FROM `user_achievements` WHERE `user_id` = @id;");
                DbCon.AddParameter("id", Player.Id);

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        this._achievements.TryAdd(Reader.GetString("group_id"), new Achievement(Reader.GetString("group_id"), Reader.GetInt32("level"), Reader.GetInt32("progress")));
                    }
                }
            }

            this._player = Player;

            return true;
        }

        public bool TryGetAchievementData(string GroupId, out Achievement Achievement)
        {
            return this._achievements.TryGetValue(GroupId, out Achievement);
        }

        // TO-DO: Update sql
        public void AddOrUpdate(string Group, int Level, int Progress)
        {
            /*using (var s = Mango.GetServer().GetDatabaseOld().GetSessionFactory().OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    var DbAchievement = s.CreateCriteria<UserAchievementEntity>()
                        .Add(Restrictions.Eq("UserId", this._player.Id))
                        .Add(Restrictions.Eq("GroupId", Group))
                        .UniqueResult<UserAchievementEntity>();

                    if (DbAchievement != null)
                    {
                        DbAchievement.Level = Level;
                        DbAchievement.Progress = Progress;

                        s.Update(DbAchievement);
                    }
                    else
                    {
                        UserAchievementEntity NewDbAchievement = new UserAchievementEntity
                        {
                            UserId = this._player.Id,
                            GroupId = Group,
                            Level = Level,
                            Progress = Progress
                        };

                        s.Save(DbAchievement);
                    }
                }
            }*/

            Achievement Achievement = null;

            if (this._achievements.TryGetValue(Group, out Achievement))
            {
                Achievement.Level = Level;
                Achievement.Progress = Progress;
            }
            else
            {
                this._achievements.TryAdd(Group, new Achievement(Group, Level, Progress));
            }
        }
    }
}
