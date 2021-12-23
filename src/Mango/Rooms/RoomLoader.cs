using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Mango.Utilities;
using System.Collections.Concurrent;
using Mango.Players;

namespace Mango.Rooms
{
    static class RoomLoader
    {
        public static List<RoomData> GetRoomsDataByOwnerIdSortByName(int OwnerId)
        {
            List<RoomData> Datas = new List<RoomData>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `rooms` WHERE `owner_id` = @ownerid ORDER BY `name`;");
                DbCon.AddParameter("ownerid", OwnerId);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        if (Mango.GetServer().GetRoomManager().TryGetRoom(Reader.GetInt32("id"), out RoomInstance Instance))
                        {
                            Datas.Add(Instance);
                        }
                        else
                        {
                            if (Mango.GetServer().GetRoomManager().TryGetModel(Reader.GetString("model"), out RoomModel Model))
                            {
                                    Datas.Add(new RoomData(Reader.GetInt32("id"), Reader.GetInt32("owner_id"), Reader.GetString("name"),
                                    Reader.GetString("description"), Reader.GetString("tags"), Reader.GetString("access_type"),
                                    Reader.GetString("password"), Reader.GetInt32("category"), Reader.GetInt32("max_users"), Reader.GetInt32("score"),
                                    Model, Reader.GetInt32("allow_pets"), Reader.GetInt32("allow_pets_eating"), Reader.GetInt32("disable_blocking"),
                                    Reader.GetInt32("hide_walls"), Reader.GetInt32("thickness_wall"), Reader.GetInt32("thickness_floor"), Reader.GetString("decorations")));
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return Datas;
        }

        public static List<RoomData> SearchForRooms(string Query, int Limit = 50)
        {
            List<RoomData> Datas = new List<RoomData>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `rooms` WHERE `name` LIKE @query OR `tags` LIKE @tquery  LIMIT " + Limit + ";");
                DbCon.AddParameter("query", Query + "%");
                DbCon.AddParameter("tquery", "%" + Query + "%");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        RoomInstance Instance = null;

                        if (Mango.GetServer().GetRoomManager().TryGetRoom(Reader.GetInt32("id"), out Instance))
                        {
                            Datas.Add(Instance);
                        }
                        else
                        {
                            RoomModel Model = null;

                            if (!Mango.GetServer().GetRoomManager().TryGetModel(Reader.GetString("model"), out Model))
                            {
                                continue;
                            }

                            Datas.Add(new RoomData(Reader.GetInt32("id"), Reader.GetInt32("owner_id"), Reader.GetString("name"),
                                Reader.GetString("description"), Reader.GetString("tags"), Reader.GetString("access_type"),
                                Reader.GetString("password"), Reader.GetInt32("category"), Reader.GetInt32("max_users"), Reader.GetInt32("score"),
                                Model, Reader.GetInt32("allow_pets"), Reader.GetInt32("allow_pets_eating"), Reader.GetInt32("disable_blocking"),
                                Reader.GetInt32("hide_walls"), Reader.GetInt32("thickness_wall"), Reader.GetInt32("thickness_floor"), Reader.GetString("decorations")));
                        }
                    }
                }
            }

            return Datas;
        }

        public static List<RoomData> SearchForRoomsByOwnerName(string OwnerName, int Limit = 50)
        {
            List<RoomData> Datas = new List<RoomData>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT `id` FROM `users` WHERE `username` = @name;");
                DbCon.AddParameter("name", OwnerName);
                DbCon.Open();

                int OwnerId = 0;

                if (!DbCon.TryExecuteSingleInt(out OwnerId))
                {
                    return Datas;
                }
                else
                {
                    DbCon.SetQuery("SELECT * FROM `rooms` WHERE `owner_id` = @id LIMIT " + Limit + ";");
                    DbCon.AddParameter("id", OwnerId);

                    using (MySqlDataReader Reader = DbCon.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            RoomInstance Instance = null;

                            if (Mango.GetServer().GetRoomManager().TryGetRoom(Reader.GetInt32("id"), out Instance))
                            {
                                Datas.Add(Instance);
                            }
                            else
                            {
                                RoomModel Model = null;

                                if (!Mango.GetServer().GetRoomManager().TryGetModel(Reader.GetString("model"), out Model))
                                {
                                    continue;
                                }

                                Datas.Add(new RoomData(Reader.GetInt32("id"), Reader.GetInt32("owner_id"), Reader.GetString("name"),
                                    Reader.GetString("description"), Reader.GetString("tags"), Reader.GetString("access_type"),
                                    Reader.GetString("password"), Reader.GetInt32("category"), Reader.GetInt32("max_users"), Reader.GetInt32("score"),
                                    Model, Reader.GetInt32("allow_pets"), Reader.GetInt32("allow_pets_eating"), Reader.GetInt32("disable_blocking"),
                                    Reader.GetInt32("hide_walls"), Reader.GetInt32("thickness_wall"), Reader.GetInt32("thickness_floor"), Reader.GetString("decorations")));
                            }
                        }
                    }
                }
            }

            return Datas;
        }

        public static List<RoomData> GetRoomsForIds(ICollection<int> RoomIds)
        {
            List<RoomData> Data = new List<RoomData>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                foreach (int Id in RoomIds)
                {
                    RoomInstance Instance = null;

                    if (Mango.GetServer().GetRoomManager().TryGetRoom(Id, out Instance))
                    {
                        Data.Add(Instance);
                    }
                    else
                    {
                        DbCon.SetQuery("SELECT * FROM `rooms` WHERE `id` = @id LIMIT 1;");
                        DbCon.AddParameter("id", Id);
                        if (!DbCon.IsOpen()) { DbCon.Open(); }

                        using (MySqlDataReader Reader = DbCon.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                RoomModel Model = null;

                                if (!Mango.GetServer().GetRoomManager().TryGetModel(Reader.GetString("model"), out Model))
                                {
                                    continue;
                                }

                                Data.Add(new RoomData(Reader.GetInt32("id"), Reader.GetInt32("owner_id"), Reader.GetString("name"),
                                    Reader.GetString("description"), Reader.GetString("tags"), Reader.GetString("access_type"),
                                    Reader.GetString("password"), Reader.GetInt32("category"), Reader.GetInt32("max_users"), Reader.GetInt32("score"),
                                    Model, Reader.GetInt32("allow_pets"), Reader.GetInt32("allow_pets_eating"), Reader.GetInt32("disable_blocking"),
                                    Reader.GetInt32("hide_walls"), Reader.GetInt32("thickness_wall"), Reader.GetInt32("thickness_floor"), Reader.GetString("decorations")));
                            }
                        }
                    }
                }
            }

            return Data;
        }

        public static bool TryGetData(int RoomId, out RoomData Data)
        {
            RoomInstance Instance = null;

            if (Mango.GetServer().GetRoomManager().TryGetRoom(RoomId, out Instance))
            {
                Data = Instance;
                return true;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `rooms` WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("id", RoomId);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        RoomModel Model = null;

                        if (!Mango.GetServer().GetRoomManager().TryGetModel(Reader.GetString("model"), out Model))
                        {
                            continue;
                        }

                        RoomData RData = new RoomData(Reader.GetInt32("id"), Reader.GetInt32("owner_id"), Reader.GetString("name"),
                            Reader.GetString("description"), Reader.GetString("tags"), Reader.GetString("access_type"),
                            Reader.GetString("password"), Reader.GetInt32("category"), Reader.GetInt32("max_users"), Reader.GetInt32("score"),
                            Model, Reader.GetInt32("allow_pets"), Reader.GetInt32("allow_pets_eating"), Reader.GetInt32("disable_blocking"),
                            Reader.GetInt32("hide_walls"), Reader.GetInt32("thickness_wall"), Reader.GetInt32("thickness_floor"), Reader.GetString("decorations"));

                        Data = RData;
                        return true;
                    }
                }
            }

            Data = null;
            return false;
        }
    }
}
