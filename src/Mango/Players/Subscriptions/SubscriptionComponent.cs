using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Subscriptions;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Players.Subscriptions
{
    class SubscriptionComponent
    {
        private readonly Dictionary<string, Subscription> _activeSubscriptions = null;

        public SubscriptionComponent()
        {
            this._activeSubscriptions = new Dictionary<string, Subscription>();
        }

        public bool Init(Player Player)
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.SetQuery("SELECT * FROM `user_subscriptions` WHERE `user_id` = @uid;");
                DbCon.AddParameter("uid", Player.Id);

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        SubscriptionData Data = null;

                        if (!Mango.GetServer().GetSubscriptionManager().TryGetSubscriptionData(Reader.GetInt32("subscription_id"), out Data))
                        {
                            using (var DbCon2 = Mango.GetServer().GetDatabase().GetConnection())
                            {
                                try
                                {
                                    DbCon2.Open();
                                    DbCon2.BeginTransaction();

                                    DbCon2.SetQuery("DELETE FROM `user_subscriptions` WHERE `id` = @id;");
                                    DbCon2.AddParameter("id", Reader.GetInt32("id"));
                                    DbCon2.ExecuteNonQuery();

                                    DbCon2.Commit();
                                }
                                catch (MySqlException) { DbCon.Rollback(); }
                            }

                            continue;
                        }

                        Subscription Subscription = new Subscription(Reader.GetInt32("id"), Reader.GetInt32("user_id"),
                            Reader.GetInt32("subscription_id"), Data, Reader.GetInt32("current_level"), Reader.GetDouble("timestamp_created"),
                            Reader.GetDouble("timestamp_expire"));

                        if (!Subscription.IsActive)
                        {
                            Subscription.Expire();
                            continue;
                        }

                        if (this._activeSubscriptions.ContainsKey(Subscription.Data.Name))
                        {
                            using (var DbCon2 = Mango.GetServer().GetDatabase().GetConnection())
                            {
                                try
                                {
                                    DbCon2.Open();
                                    DbCon2.BeginTransaction();

                                    DbCon2.SetQuery("DELETE FROM `user_subscriptions` WHERE `id` = @id;");
                                    DbCon2.AddParameter("id", Reader.GetInt32("id"));
                                    DbCon2.ExecuteNonQuery();

                                    DbCon2.Commit();
                                }
                                catch (MySqlException) { DbCon.Rollback(); }
                            }

                            continue;
                        }

                        this._activeSubscriptions.Add(Subscription.Data.Name, Subscription);
                    }
                }
            }

            return true;
        }

        public bool TryAddSubscription(string Name, Subscription Subscription)
        {
            if (this._activeSubscriptions.ContainsKey(Name))
            {
                return false;
            }

            this._activeSubscriptions.Add(Name, Subscription);
            return true;
        }

        public ICollection<Subscription> Subscriptions
        {
            get
            {
                return this._activeSubscriptions.Values;
            }
        }

        public bool TryGetSubscription(string Name, out Subscription Subscription)
        {
            return this._activeSubscriptions.TryGetValue(Name, out Subscription);
        }
    }
}
