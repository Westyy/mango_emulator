using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Effects
{
    /// <summary>
    /// Stores the effects for the Players Avatar.
    /// </summary>
    sealed class EffectsComponent
    {
        /// <summary>
        /// Effects stored by ID > Effect.
        /// </summary>
        private readonly ConcurrentDictionary<int, AvatarEffect> _effects = new ConcurrentDictionary<int, AvatarEffect>();
        
        public EffectsComponent()
        {
        }

        /// <summary>
        /// Initializes the EffectsComponent.
        /// </summary>
        /// <param name="UserId"></param>
        public bool Init(Player Player)
        {
            if (_effects.Count > 0)
            {
                throw new InvalidOperationException("Cannot re-initialize the effects componenet.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.SetQuery("SELECT * FROM `avatar_effects` WHERE `user_id` = @id;");
                    DbCon.AddParameter("id", Player.Id);

                    using (MySqlDataReader Reader = DbCon.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            int Id = Reader.GetInt32("id");
                            int UserId = Reader.GetInt32("user_id");
                            int SpriteId = Reader.GetInt32("sprite_id");
                            double Duration = Reader.GetDouble("duration");
                            bool Activated = Reader.GetString("activated") == "1" ? true : false;
                            double TimestampActivated = Reader.GetDouble("timestamp_activated");
                            int Quantity = Reader.GetInt32("quantity");

                            if (this._effects.TryAdd(Id, new AvatarEffect(Id, UserId, SpriteId, Duration, Activated, TimestampActivated, Quantity)))
                            {
                                // dno?
                            }
                        }
                    }
                }
                catch (MySqlException)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryAdd(AvatarEffect Effect)
        {
            return this._effects.TryAdd(Effect.Id, Effect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public bool HasEffect(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            return (GetEffectNullable(SpriteId, ActivatedOnly, UnactivatedOnly) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public AvatarEffect GetEffectNullable(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            foreach (AvatarEffect Effect in this._effects.Values)
            {
                if (!Effect.HasExpired && Effect.SpriteId == SpriteId && (!ActivatedOnly || Effect.Activated) && (!UnactivatedOnly || !Effect.Activated))
                {
                    return Effect;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Player"></param>
        public void CheckEffectExpiry(Player Player)
        {
            foreach (AvatarEffect Effect in this._effects.Values)
            {
                if (Effect.HasExpired)
                {
                    Effect.HandleExpiration(Player);
                }
            }
        }

        public ICollection<AvatarEffect> GetAllEffects
        {
            get { return this._effects.Values; }
        }

        /// <summary>
        /// Disposes the EffectsComponent.
        /// </summary>
        public void Dispose()
        {
            this._effects.Clear();
        }
    }
}
