using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango
{
    static class MangoDbCreator
    {
        private static ILog log = LogManager.GetLogger("Mango.MangoDbCreator");

        /// <summary>
        /// Table engine your MySql supports and your tables use.
        /// </summary>
        private const string TABLE_ENGINE = "InnoDB";

        public static void CheckAndCreateNew()
        {
            log.Info("Updating/creating database, please wait the server will come online automatically.");

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.BeginTransaction();

                try
                {
                    DbCon.SetQuery(MangoServerStatusTable());
                    DbCon.ExecuteNonQuery();

                    // Finally
                    DbCon.Commit();
                }
                catch (MySqlException ex)
                {
                    log.Error("Failed to update/create new tables.", ex);
                }
            }

            log.Info("Database finished updating/creating.");
        }

        private static string MangoServerStatusTable()
        {
            return "CREATE TABLE IF NOT EXISTS `mango_serverstatus` ("
                + "  `key` varchar(50) NOT NULL,"
                + "  `value` varchar(50) NOT NULL,"
                + "  PRIMARY KEY (`key`)"
                + ") ENGINE=" + TABLE_ENGINE + " DEFAULT CHARSET=latin1;";
        }
    }
}
