using log4net;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mango.Players
{
    static class SSOAuthenticator
    {
        private static ILog log = LogManager.GetLogger("Mango.Players.SSOAuthenticator");

        /// <summary>
        /// The minimum length an SSO Ticket can be generated.
        /// </summary>
        private const int SSO_MIN_LENGTH = 0;

        /// <summary>
        /// Check the IP Address matches the session attempting to authenticate.
        /// </summary>
        private const bool CHECK_IP = true;

        public static bool TryAuthenticate(string SSOTicket, string IPAddress, out PlayerData Data)
        {
            SSOTicket = SSOTicket.Trim();

            if (SSOTicket.Length < SSO_MIN_LENGTH)
            {
                Data = null;
                return false;
            }

            PlayerData GeneratedData = null;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `users` WHERE `auth_ticket` = @ticket LIMIT 1;");
                DbCon.AddParameter("ticket", SSOTicket);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            GeneratedData = new PlayerData(Reader.GetInt32("id"), Reader.GetInt32("pm_level"), Reader.GetString("auth_ticket"),
                                Reader.GetString("username"), Reader.GetString("real_name"), Reader.GetString("figure"), Reader.GetString("motto"),
                                Reader.GetString("gender"), Reader.GetInt32("credits"), Reader.GetInt32("pixels"), Reader.GetInt32("home_room"),
                                Reader.GetInt32("score"), Reader.GetInt32("accept_friend_requests"), Reader.GetInt32("config_volume"), Reader.GetInt32("respect_points"),
                                Reader.GetInt32("respect_left_player"), Reader.GetInt32("respect_left_pet"), Reader.GetString("tags"), Reader.GetInt32("mod_tickets"),
                                Reader.GetInt32("mod_tickets_abusive"), Reader.GetDouble("mod_tickets_cooldown"), Reader.GetInt32("mod_bans"), Reader.GetInt32("mod_cautions"),
                                Reader.GetDouble("mod_muted_until_timestamp"), Reader.GetDouble("timestamp_lastvisit"), Reader.GetDouble("timestamp_registered"), Reader.GetDouble("pixels_last_updated"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Cannot load PlayerData", ex);

                            Data = null;
                            return false;
                        }
                    }
                }
            }

            if (GeneratedData == null)
            {
                Data = null;
                return false;
            }

            // Delete the SSOTicket
#if RELEASE
            RemoveSSOTicket(GeneratedData.Id);
#endif

            if (CHECK_IP) { } // TODO

            Data = GeneratedData;
            return true;
        }

        /// <summary>
        /// Emptys the Auth Ticket field in the database.
        /// </summary>
        /// <param name="Id">UserId.</param>
        private static void RemoveSSOTicket(int Id)
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("UPDATE `users` SET `auth_ticket` = '' WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("id", Id);
                DbCon.Open();

                DbCon.BeginTransaction();
                DbCon.ExecuteNonQuery();
                DbCon.Commit();
            }
        }
    }
}
