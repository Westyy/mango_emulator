using Mango.Collections;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Database
{
    sealed class DatabaseManager
    {
        /// <summary>
        /// Output how long each separate query takes
        /// </summary>
        public const bool SHOW_QUERY_TIME = false;

        private readonly string _conStr;
        private readonly ObjectPool<DatabaseConnection> _conPool;

        public DatabaseManager(string ConnectionStr)
        {
            this._conStr = ConnectionStr;
            this._conPool = new ObjectPool<DatabaseConnection>(() => new DatabaseConnection(this._conStr, this._conPool));
        }

        public bool TestConnection()
        {
            try
            {
                using (var DbCon = GetConnection())
                {
                    DbCon.Open();
                    DbCon.SetQuery("SELECT 1+1;");
                    DbCon.ExecuteNonQuery();
                }
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        public DatabaseConnection GetConnection()
        {
            //return this._conPool.GetObject();
            return new DatabaseConnection(this._conStr, this._conPool);
        }
    }
}
