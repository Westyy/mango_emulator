using Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Mango.Communication.Sessions;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Effects
{
    sealed class AvatarEffect
    {
        private int _id;
        private int _userId;
        private int _spriteId;
        private double _duration;
        private bool _activated;
        private double _timestampActivated;
        private int _quantity;

        public AvatarEffect(int Id, int UserId, int SpriteId, double Duration, bool Activated, double TimestampActivated, int Quantity)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.SpriteId = SpriteId;
            this.Duration = Duration;
            this.Activated = Activated;
            this.TimestampActivated = TimestampActivated;
            this.Quantity = Quantity;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int UserId
        {
            get { return this._userId; }
            set { this._userId = value; }
        }

        public int SpriteId
        {
            get { return this._spriteId; }
            set { this._spriteId = value; }
        }

        public double Duration
        {
            get { return this._duration; }
            set { this._duration = value; }
        }

        public bool Activated
        {
            get { return this._activated; }
            set { this._activated = value; }
        }

        public double TimestampActivated
        {
            get { return this._timestampActivated; }
            set { this._timestampActivated = value; }
        }

        public int Quantity
        {
            get { return this._quantity; }
            set { this._quantity = value; }
        }

        public double TimeUsed
        {
            get
            {
                return (UnixTimestamp.GetNow() - this._timestampActivated);
            }
        }

        public double TimeLeft
        {
            get
            {
                double tl = (this._activated ? this._duration - TimeUsed : this._duration);

                if (tl < 0)
                {
                    tl = 0;
                }

                return tl;
            }
        }

        public bool HasExpired
        {
            get
            {
                return (this._activated && TimeLeft <= 0);
            }
        }

        /// <summary>
        /// Activates the AvatarEffect
        /// </summary>
        public bool Activate()
        {
            double TsNow = UnixTimestamp.GetNow();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("UPDATE `avatar_effects` SET `activated` = '1', `timestamp_activated` = @ts WHERE `id` = @id;");
                    DbCon.AddParameter("ts", TsNow);
                    DbCon.AddParameter("id", this.Id);
                    DbCon.ExecuteNonQuery();

                    this._activated = true;
                    this._timestampActivated = TsNow;

                    DbCon.Commit();
                    return true;
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return false;
                }
            }
        }

        public void HandleExpiration(Player Player)
        {
            this._quantity--;

            this._activated = false;
            this._timestampActivated = 0;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    if (this._quantity < 1)
                    {
                        DbCon.SetQuery("DELETE FROM `avatar_effects` WHERE `id` = @id;");
                        DbCon.AddParameter("id", this.Id);
                        DbCon.ExecuteNonQuery();
                    }
                    else
                    {
                        DbCon.SetQuery("UPDATE `avatar_effects` SET `quantity` = @qt, `activated` = '0', `timestamp_activated` = 0 WHERE `id` = @id;");
                        DbCon.AddParameter("qt", this.Quantity);
                        DbCon.AddParameter("id", this.Id);
                        DbCon.ExecuteNonQuery();
                    }

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                }
            }

            Player.GetSession().SendPacket(new AvatarEffectExpiredComposer(this));

            // reset fx if in room?
        }

        public void AddToQuantity()
        {
            this._quantity++;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("UPDATE `avatar_effects` SET `quantity` = @qt, WHERE `id` = @id;");
                    DbCon.AddParameter("qt", this.Quantity);
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
    }
}
