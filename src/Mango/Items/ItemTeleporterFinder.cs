using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    static class ItemTeleporterFinder
    {
        private static Dictionary<int, int> TeleLinkCache = new Dictionary<int, int>();

        public static int GetValue(int LinkedItemId)
        {
            return (TeleLinkCache.ContainsKey(LinkedItemId) ? TeleLinkCache[LinkedItemId] : 0);
        }

        public static void Add(int LinkedItemId)
        {
            if (TeleLinkCache.ContainsKey(LinkedItemId))
            {
                TeleLinkCache.Remove(LinkedItemId);
            }

            int RoomId = 0;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.SetQuery("SELECT `room_id` FROM `items` WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("id", LinkedItemId);

                try
                {
                    RoomId = DbCon.ExecuteSingleInt();
                }
                catch (MySqlException)
                {
                    MangoDebugger.ConsoleOut("Teleporter Error"); // debugging
                }
            }

            if (RoomId > 0)
            {
                TeleLinkCache.Add(LinkedItemId, RoomId);
            }
        }
    }
}
