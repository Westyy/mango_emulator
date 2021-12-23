using System.Collections.Generic;
using Mango.Utilities;
using System;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;
using log4net;
using System.Data;

namespace Mango.Players
{
    static class PlayerLoader
    {
        private static ILog log = LogManager.GetLogger("Mango.Players.PlayerLoader");

        /// <summary>
        /// Gets the Players Username by there Id.
        /// </summary>
        /// <param name="UserId">User Id.</param>
        /// <returns>The players username otherwise "Unknown Username" if nothing found.</returns>
        public static string GetPlayerNameById(int UserId)
        {
            PlayerData Info = GetDataById(UserId);

            if (Info != null)
            {
                return Info.Username;
            }
            else
            {
                return "Unknown Username";
            }
        }

        /// <summary>
        /// Searches players by there username with a limit.
        /// </summary>
        /// <param name="SearchQuery">Username to search for (LIKE).</param>
        /// <param name="MaxResults">Maximum results to return.</param>
        /// <returns>A list of found results.</returns>
        public static List<PlayerData> SearchPlayersByUsernameLike(string SearchQuery, int MaxResults = 50)
        {
            List<PlayerData> Players = new List<PlayerData>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `users` WHERE `username` LIKE @query LIMIT " + MaxResults + ";");
                DbCon.AddParameter("query", "%" + SearchQuery + "%");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            Players.Add(new PlayerData(Reader.GetInt32("id"), Reader.GetInt32("pm_level"), Reader.GetString("auth_ticket"),
                                Reader.GetString("username"), Reader.GetString("figure"), Reader.GetString("motto"),
                                Reader.GetString("gender"), Reader.GetInt32("credits"), Reader.GetInt32("pixels"), Reader.GetInt32("home_room"),
                                Reader.GetInt32("score"), Reader.GetInt32("accept_friend_requests"), Reader.GetInt32("config_volume")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Cannot load PlayerData", ex);
                            continue;
                        }
                    }
                }
            }

            return Players;
        }

        /// <summary>
        /// Finds a playrs data by there Player/User Id.
        /// </summary>
        /// <param name="PlayerId">Player/User Id.</param>
        /// <returns>Null if not found or PlayerData if exists.</returns>
        public static PlayerData GetDataById(int PlayerId)
        {
            Player Player = null;

            if (Mango.GetServer().GetPlayerManager().TryGet(PlayerId, out Player))
            {
                return Player;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("id", PlayerId);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            return new PlayerData(Reader.GetInt32("id"), Reader.GetInt32("pm_level"), Reader.GetString("auth_ticket"),
                                Reader.GetString("username"), Reader.GetString("figure"), Reader.GetString("motto"),
                                Reader.GetString("gender"), Reader.GetInt32("credits"), Reader.GetInt32("pixels"), Reader.GetInt32("home_room"),
                                Reader.GetInt32("score"), Reader.GetInt32("accept_friend_requests"), Reader.GetInt32("config_volume"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Cannot load PlayerData", ex);
                            return null;
                        }
                    }
                }
            }

            return null;
        }

        public static PlayerData GetInfoByUsername(string PlayerUsername)
        {
            Player Player = null;

            if (Mango.GetServer().GetPlayerManager().TryGet(PlayerUsername, out Player))
            {
                return Player;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `users` WHERE `username` = @username LIMIT 1;");
                DbCon.AddParameter("username", PlayerUsername);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            return new PlayerData(Reader.GetInt32("id"), Reader.GetInt32("pm_level"), Reader.GetString("auth_ticket"),
                                Reader.GetString("username"), Reader.GetString("figure"), Reader.GetString("motto"),
                                Reader.GetString("gender"), Reader.GetInt32("credits"), Reader.GetInt32("pixels"), Reader.GetInt32("home_room"),
                                Reader.GetInt32("score"), Reader.GetInt32("accept_friend_requests"), Reader.GetInt32("config_volume"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Cannot load PlayerData", ex);
                            return null;
                        }
                    }
                }
            }

            return null;
        }
    }
}
