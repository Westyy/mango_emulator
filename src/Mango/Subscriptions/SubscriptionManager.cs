using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Subscriptions
{
    class SubscriptionManager
    {
        private static ILog log = LogManager.GetLogger("Mango.Subscriptions.SubscriptionManager");

        private readonly Dictionary<string, SubscriptionData> Subscriptions = new Dictionary<string, SubscriptionData>();

        private readonly Dictionary<int, string> SubscriptionIdToName = new Dictionary<int, string>();

        public SubscriptionManager()
        {
        }

        public void Init()
        {
            if (Subscriptions.Count > 0)
            {
                throw new InvalidOperationException("Subscriptions are already initialized.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `subscriptions`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            string Name = Reader.GetString("name");

                            this.Subscriptions.Add(Name, new SubscriptionData(Reader.GetInt32("id"), Name, Reader.GetInt32("levels")));
                            this.SubscriptionIdToName.Add(Reader.GetInt32("id"), Name);
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Subscription for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + this.Subscriptions.Count + " subscriptions.");
        }

        public bool TryGetSubscriptionData(int Id, out SubscriptionData Data)
        {
            string Name = string.Empty;

            if (this.SubscriptionIdToName.TryGetValue(Id, out Name))
            {
                SubscriptionData DataVal = null;

                if (this.Subscriptions.TryGetValue(Name, out DataVal))
                {
                    Data = DataVal;
                    return true;
                }
            }

            Data = null;
            return false;
        }

        public bool TryGetSubscriptionData(string Name, out SubscriptionData Data)
        {
            return this.Subscriptions.TryGetValue(Name, out Data);
        }
    }
}
