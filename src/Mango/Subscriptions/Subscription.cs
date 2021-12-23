using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;
using MySql.Data.MySqlClient;

namespace Mango.Subscriptions
{
    sealed class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionData Data { get; set; }
        public int CurrentLevel { get; set; }
        public double TimestampCreated { get; set; }
        public double TimestampExpires { get; set; }

        public Subscription(int Id, int UserId, int SubscriptionId, SubscriptionData Data, int CurrentLevel,
            double Created, double Expires)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.SubscriptionId = SubscriptionId;
            this.Data = Data;
            this.CurrentLevel = CurrentLevel;
            this.TimestampCreated = Created;
            this.TimestampExpires = Expires;
        }

        public bool IsActive
        {
            get
            {
                return (UnixTimestamp.GetNow() < TimestampExpires);
            }
        }

        public double TimeLeft
        {
            get
            {
                return Math.Round((UnixTimestamp.GetNow() - TimestampExpires));
            }
        }

        public void Expire()
        {
            TimestampCreated = 0;
            TimestampExpires = 0;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("DELETE FROM `user_subscriptions` WHERE `id` = @id;");
                    DbCon.AddParameter("id", this.Id);

                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                }
            }
        }

        /*public void Extend(int Level, double Timestamp) // use the 'factory'
        {
            if (!IsActive)
            {
                this.TimestampCreated = UnixTimestamp.GetNow();
                this.TimestampExpires = UnixTimestamp.GetNow();
            }

            this.TimestampExpires += Timestamp;

            if (this.CurrentLevel != Level)
            {
                this.CurrentLevel = Level;
            }

            using (var s = Mango.GetServer().GetDatabase().GetSessionFactory().OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    s.SaveOrUpdate(this);
                    tx.Commit();
                }
            }
        }*/
    }
}
