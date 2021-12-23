using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;

namespace Mango.Quests
{
    sealed class QuestManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Quests.QuestManager");

        private readonly Dictionary<int, Quest> _quests = new Dictionary<int, Quest>();

        public QuestManager()
        {
        }

        public void Init()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `quests`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._quests.Add(Reader.GetInt32("id"), new Quest(Reader.GetInt32("id"), Reader.GetString("category"), Reader.GetInt32("series_number"), Reader.GetInt32("goal_type"),
                                Reader.GetInt32("goal_data"), Reader.GetString("name"), Reader.GetInt32("reward"), Reader.GetString("data_bit")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Quest for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this._quests.Count + " quests.");
        }

        public bool TryGetQuest(int QuestId, out Quest Quest)
        {
            return this._quests.TryGetValue(QuestId, out Quest);
        }

        public ICollection<Quest> QuestList
        {
            get
            {
                return this._quests.Values;
            }
        }
    }
}
