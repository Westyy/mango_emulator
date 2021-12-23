using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Effects
{
    static class AvatarEffectFactory
    {
        /// <summary>
        /// Creates a new AvatarEffect with the specified details.
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="SpriteId"></param>
        /// <param name="Duration"></param>
        /// <returns></returns>
        public static AvatarEffect CreateNullable(Player Player, int SpriteId, double Duration)
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("INSERT INTO `avatar_effects` (user_id,sprite_id,duration,activated,timestamp_activated,quantity) VALUES(@uid,@sid,@dur,'0',0,1);");
                    DbCon.AddParameter("uid", Player.Id);
                    DbCon.AddParameter("sid", SpriteId);
                    DbCon.AddParameter("dur", Duration);
                    DbCon.ExecuteNonQuery();

                    AvatarEffect Effect = new AvatarEffect(DbCon.SelectLastId(), Player.Id, SpriteId, Duration, false, 0, 1);

                    DbCon.Commit();
                    return Effect;
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return null;
                }
            }
        }
    }
}
