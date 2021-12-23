using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using Mango.Rooms.Mapping;

namespace Mango.Items
{
    static class ItemLoader
    {
        public static List<Item> GetItemsForRoom(int RoomId)
        {
            List<Item> I = new List<Item>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.SetQuery("SELECT * FROM `items` WHERE `room_id` = @rid AND `user_id` = 0;");
                    DbCon.AddParameter("rid", RoomId);

                    using (MySqlDataReader Reader = DbCon.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            ItemData Data = null;

                            if (Mango.GetServer().GetItemDataManager().GetItem(Reader.GetInt32("definition_id"), out Data))
                            {
                                I.Add(new Item(Reader.GetInt32("id"), Data, Reader.GetInt32("definition_id"), Reader.GetInt32("user_id"),
                                    Reader.GetInt32("room_id"), Reader.GetString("room_wall_pos"), Reader.GetInt32("room_rot"),
                                    new Vector3D(Reader.GetInt32("room_pos_x"), Reader.GetInt32("room_pos_y"), Reader.GetDouble("room_pos_z")), Reader.GetString("flags"),
                                    Reader.GetString("flags_display"), Reader.GetInt32("untradable") == 1 ? true : false, Reader.GetDouble("expire_timestamp"),
                                    Reader.GetInt32("soundmanager_id"), Reader.GetInt32("soundmanager_order")));
                            }
                            else
                            {
                                // Item data does not exist anymore.
                            }
                        }
                    }
                }
                catch (MySqlException)
                {

                }
            }

            return I;
        }

        public static List<Item> GetItemsForUser(int UserId)
        {
            List<Item> I = new List<Item>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.SetQuery("SELECT * FROM `items` WHERE `room_id` = 0 AND `user_id` = @uid;");
                    DbCon.AddParameter("uid", UserId);

                    using (MySqlDataReader Reader = DbCon.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            ItemData Data = null;

                            if (Mango.GetServer().GetItemDataManager().GetItem(Reader.GetInt32("definition_id"), out Data))
                            {
                                I.Add(new Item(Reader.GetInt32("id"), Data, Reader.GetInt32("definition_id"), Reader.GetInt32("user_id"),
                                    Reader.GetInt32("room_id"), Reader.GetString("room_wall_pos"), Reader.GetInt32("room_rot"),
                                    new Vector3D(Reader.GetInt32("room_pos_x"), Reader.GetInt32("room_pos_y"), Reader.GetDouble("room_pos_z")), Reader.GetString("flags"),
                                    Reader.GetString("flags_display"), Reader.GetInt32("untradable") == 1 ? true : false, Reader.GetDouble("expire_timestamp"),
                                    Reader.GetInt32("soundmanager_id"), Reader.GetInt32("soundmanager_order")));
                            }
                            else
                            {
                                // Item data does not exist anymore.
                            }
                        }
                    }
                }
                catch (MySqlException)
                {

                }
            }

            return I;
        }
    }
}
