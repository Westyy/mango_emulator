using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Utilities;
using MySql.Data.MySqlClient;

namespace Mango.Subscriptions
{
    static class SubscriptionFactory
    {
        public static bool AddOrExtend(Player Player, string Subscription, int Level, double ExtendTimestamp, out Subscription Sub)
        {
            Subscription ActiveSubscription = null;

            if (Player.GetSubscriptions().TryGetSubscription(Subscription, out ActiveSubscription))
            {
                if (ActiveSubscription.Data == null) // no active subscription data
                {
                    using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                    {
                        try
                        {
                            DbCon.Open();
                            DbCon.BeginTransaction();

                            DbCon.SetQuery("DELETE FROM `user_subscriptions` WHERE `id` = @id LIMIT 1;");
                            DbCon.AddParameter("id", ActiveSubscription.Id);
                            DbCon.ExecuteNonQuery();

                            DbCon.Commit();
                        }
                        catch (MySqlException)
                        {
                            DbCon.Rollback();
                        }
                    }

                    Sub = null;
                    return false;
                }

                if (Level > ActiveSubscription.Data.Levels) // level cannot be higher than the maximum amount of sub levels
                {
                    Sub = null;
                    return false;
                }

                if (!ActiveSubscription.IsActive) // correct timestamps if we have expired already
                {
                    ActiveSubscription.TimestampCreated = UnixTimestamp.GetNow();
                    ActiveSubscription.TimestampExpires = UnixTimestamp.GetNow();
                }

                ActiveSubscription.TimestampExpires += ExtendTimestamp;
                ActiveSubscription.CurrentLevel = Level;

                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("UPDATE `user_subscriptions` SET `current_level` = @level, `timestamp_expire` = @expire WHERE `id` = @id LIMIT 1;");
                        DbCon.AddParameter("level", ActiveSubscription.CurrentLevel);
                        DbCon.AddParameter("expire", ActiveSubscription.TimestampExpires);
                        DbCon.AddParameter("id", ActiveSubscription.Id);
                        DbCon.ExecuteNonQuery();

                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }

                Sub = ActiveSubscription;
                return true;
            }
            else
            {
                SubscriptionData Data = null;

                if (!Mango.GetServer().GetSubscriptionManager().TryGetSubscriptionData(Subscription, out Data))
                {
                    Sub = null;
                    return false; // no such subscription exists
                }

                Subscription NewSub = null;

                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    try
                    {
                        double TsNow = UnixTimestamp.GetNow();
                        double TsExpire = TsNow += ExtendTimestamp;

                        DbCon.SetQuery("INSERT INTO `user_subscriptions` (user_id,subscription_id,current_level,timestamp_created,timestamp_expire) VALUES(@uid,@sid,@level,@created,@expire);");
                        DbCon.AddParameter("uid", Player.Id);
                        DbCon.AddParameter("sid", Data.Id);
                        DbCon.AddParameter("level", Level);
                        DbCon.AddParameter("created", TsNow);
                        DbCon.AddParameter("expire", TsExpire);
                        DbCon.ExecuteNonQuery();

                        NewSub = new Subscription(DbCon.SelectLastId(), Player.Id, Data.Id, Data, Level, TsNow, TsExpire);

                        if (Player.GetSubscriptions().TryAddSubscription(Subscription, NewSub))
                        {
                            DbCon.Commit();
                        }
                        else
                        {
                            Sub = null;
                            return false;
                        }
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                        Sub = null;
                        return false;
                    }
                }

                Sub = NewSub;
                return true;
            }
        }
    }
}
