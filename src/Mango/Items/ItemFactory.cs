using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using MySql.Data.MySqlClient;
using log4net;
using Mango.Rooms.Mapping;

namespace Mango.Items
{
    static class ItemFactory
    {
        private static ILog log = LogManager.GetLogger("Mango.Items.ItemFactory");

        public static Item CreateSingleItemNullable(ItemData Data, Player Player, string Flags, string DisplayFlags, double ExpireTimestamp, bool Untradable = false)
        {
            if (Data == null) throw new InvalidOperationException("Data cannot be null.");

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("INSERT INTO `items` (definition_id,user_id,room_id,room_pos_x,room_pos_y,room_pos_z,room_wall_pos,room_rot,flags,flags_display,untradable,expire_timestamp,soundmanager_id,soundmanager_order) VALUES(@did,@uid,@rid,@x,@y,@z,@wallpos,@rot,@flags,@dflags,@untradable,@expirets,@soundid,@soundorder);");
                    DbCon.AddParameter("did", Data.Id);
                    DbCon.AddParameter("uid", Player.Id);
                    DbCon.AddParameter("rid", 0);
                    DbCon.AddParameter("x", 0);
                    DbCon.AddParameter("y", 0);
                    DbCon.AddParameter("z", 0);
                    DbCon.AddParameter("wallpos", "");
                    DbCon.AddParameter("rot", 0);
                    DbCon.AddParameter("flags", Flags);
                    DbCon.AddParameter("dflags", DisplayFlags);
                    DbCon.AddParameter("untradable", Untradable == true ? "1" : "0");
                    DbCon.AddParameter("expirets", ExpireTimestamp);
                    DbCon.AddParameter("soundid", 0);
                    DbCon.AddParameter("soundorder", 0);
                    DbCon.ExecuteNonQuery();

                    Item Item = new Item(DbCon.SelectLastId(), Data, Data.Id, Player.Id, 0, "", 0, new Vector3D(), Flags, DisplayFlags, Untradable, ExpireTimestamp, 0, 0);

                    DbCon.Commit();

                    return Item;
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return null;
                }
            }
        }

        public static List<Item> CreateMultipleItems(ItemData Data, Player Player, string Flags, string DisplayFlags, double ExpireTimestamp, int Amount, bool Untradable = false)
        {
            if (Data == null) throw new InvalidOperationException("Data cannot be null.");

            List<Item> Items = new List<Item>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    for (int i = 0; i < Amount; i++)
                    {
                        DbCon.SetQuery("INSERT INTO `items` (definition_id,user_id,room_id,room_pos_x,room_pos_y,room_pos_z,room_wall_pos,room_rot,flags,flags_display,untradable,expire_timestamp,soundmanager_id,soundmanager_order) VALUES(@did,@uid,@rid,@x,@y,@z,@wallpos,@rot,@flags,@dflags,@untradable,@expirets,@soundid,@soundorder);");
                        DbCon.AddParameter("did", Data.Id);
                        DbCon.AddParameter("uid", Player.Id);
                        DbCon.AddParameter("rid", 0);
                        DbCon.AddParameter("x", 0);
                        DbCon.AddParameter("y", 0);
                        DbCon.AddParameter("z", 0);
                        DbCon.AddParameter("wallpos", "");
                        DbCon.AddParameter("rot", 0);
                        DbCon.AddParameter("flags", Flags);
                        DbCon.AddParameter("dflags", DisplayFlags);
                        DbCon.AddParameter("untradable", Untradable == true ? "1" : "0");
                        DbCon.AddParameter("expirets", ExpireTimestamp);
                        DbCon.AddParameter("soundid", 0);
                        DbCon.AddParameter("soundorder", 0);
                        DbCon.ExecuteNonQuery();

                        Item Item = new Item(DbCon.SelectLastId(), Data, Data.Id, Player.Id, 0, "", 0, new Vector3D(), Flags, DisplayFlags, Untradable, ExpireTimestamp, 0, 0);
                        Items.Add(Item);
                    }

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return null;
                }
            }

            return Items;
        }

        public static List<Item> CreateTeleporterItems(ItemData Data, Player Player, double ExpireTimestamp, bool Untradable = false)
        {
            List<Item> Items = new List<Item>();

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("INSERT INTO `items` (definition_id,user_id,room_id,room_pos_x,room_pos_y,room_pos_z,room_wall_pos,room_rot,flags,flags_display,untradable,expire_timestamp,soundmanager_id,soundmanager_order) VALUES(@did,@uid,@rid,@x,@y,@z,@wallpos,@rot,@flags,@dflags,@untradable,@expirets,@soundid,@soundorder);");
                    DbCon.AddParameter("did", Data.Id);
                    DbCon.AddParameter("uid", Player.Id);
                    DbCon.AddParameter("rid", 0);
                    DbCon.AddParameter("x", 0);
                    DbCon.AddParameter("y", 0);
                    DbCon.AddParameter("z", 0);
                    DbCon.AddParameter("wallpos", "");
                    DbCon.AddParameter("rot", 0);
                    DbCon.AddParameter("flags", "");
                    DbCon.AddParameter("dflags", "");
                    DbCon.AddParameter("untradable", Untradable == true ? "1" : "0");
                    DbCon.AddParameter("expirets", ExpireTimestamp);
                    DbCon.AddParameter("soundid", 0);
                    DbCon.AddParameter("soundorder", 0);
                    DbCon.ExecuteNonQuery();

                    int Item1Id = DbCon.SelectLastId();

                    DbCon.SetQuery("INSERT INTO `items` (definition_id,user_id,room_id,room_pos_x,room_pos_y,room_pos_z,room_wall_pos,room_rot,flags,flags_display,untradable,expire_timestamp,soundmanager_id,soundmanager_order) VALUES(@did,@uid,@rid,@x,@y,@z,@wallpos,@rot,@flags,@dflags,@untradable,@expirets,@soundid,@soundorder);");
                    DbCon.AddParameter("did", Data.Id);
                    DbCon.AddParameter("uid", Player.Id);
                    DbCon.AddParameter("rid", 0);
                    DbCon.AddParameter("x", 0);
                    DbCon.AddParameter("y", 0);
                    DbCon.AddParameter("z", 0);
                    DbCon.AddParameter("wallpos", "");
                    DbCon.AddParameter("rot", 0);
                    DbCon.AddParameter("flags", Item1Id.ToString());
                    DbCon.AddParameter("dflags", "");
                    DbCon.AddParameter("untradable", Untradable == true ? "1" : "0");
                    DbCon.AddParameter("expirets", ExpireTimestamp);
                    DbCon.AddParameter("soundid", 0);
                    DbCon.AddParameter("soundorder", 0);
                    DbCon.ExecuteNonQuery();

                    int Item2Id = DbCon.SelectLastId();

                    Item Item1 = new Item(Item1Id, Data, Data.Id, Player.Id, 0, "", 0, new Vector3D(), Item2Id.ToString(), "", Untradable, ExpireTimestamp, 0, 0);
                    Item Item2 = new Item(Item2Id, Data, Data.Id, Player.Id, 0, "", 0, new Vector3D(), Item1Id.ToString(), "", Untradable, ExpireTimestamp, 0, 0);

                    DbCon.SetQuery("UPDATE `items` SET `flags` = @flags WHERE `id` = @id;");
                    DbCon.AddParameter("flags", Item2.Id.ToString());
                    DbCon.AddParameter("id", Item1.Id);
                    DbCon.ExecuteNonQuery();

                    DbCon.Commit();

                    Items.Add(Item1);
                    Items.Add(Item2);
                }
                catch (MySqlException ex)
                {
                    log.Fatal("Failed to create Teleporter Item", ex);
                    DbCon.Rollback();
                    return null;
                }
            }

            return Items;
        }
    }
}
