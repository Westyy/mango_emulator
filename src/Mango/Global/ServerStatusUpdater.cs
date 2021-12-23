using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mango.Global
{
    /// <summary>
    /// Updates server status
    /// </summary>
    sealed class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 60;

        private Timer _timer;

        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            log.Info("Server Status Updater has been started.");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            int OnlineUsers = Mango.GetServer().GetPlayerManager().Count;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.SetQuery("UPDATE `mango_serverstatus` SET `value` = @users WHERE `key` = 'online' LIMIT 1;");
                DbCon.AddParameter("users", OnlineUsers);
                DbCon.BeginTransaction();

                try
                {
                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                }
            }
        }

        public void Dispose()
        {
            this._timer.Dispose();
        }
    }
}
