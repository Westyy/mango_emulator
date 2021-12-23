using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Catalog;
using Mango.Items;
using System.Diagnostics;

namespace Mango
{
    class Mango
    {
        private static MangoServer _server = null;

        private static DateTime _started = DateTime.Now;

        public static DateTime ServerStarted
        {
            get
            {
                return _started;
            }
        }

        /// <summary>
        /// Current Version of the server.
        /// </summary>
        public static string Version
        {
            get
            {
                return "0.6.6.3";
            }
        }

        /// <summary>
        /// Testing property enables/disables some debugging features.
        /// </summary>
        public static bool IsTesting
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Is a debugger currently attached to the server.
        /// </summary>
        public static bool IsDebugging
        {
            get
            {
                return Debugger.IsAttached;
            }
        }

        public Mango(MangoConfiguration Cfg, string[] Args)
        {
            _server = new MangoServer(Cfg);
            _server.Initialize();

            if (Args.Length > 0)
            {
                if (Args[0] == "/createdb")
                {
                    MangoDbCreator.CheckAndCreateNew();
                }
                else if (Args[0] == "/clearlogs")
                {
                    log4net.LogManager.Shutdown();

                    if (System.IO.File.Exists(@"logs\MangoServer.log"))
                    {
                        System.IO.File.Delete(@"logs\MangoServer.log");
                    }

                    if (System.IO.File.Exists(@"logs\errors\MangoServer_ErrorLog.log"))
                    {
                        System.IO.File.Delete(@"logs\errors\MangoServer_ErrorLog.log");
                    }

                    Console.WriteLine();
                    Console.WriteLine("***** LOG FILES DELETED *****");
                    Console.WriteLine("Mango Server will now quit, please restart, press any key to continue.");
                    Console.WriteLine();
                    Console.ReadKey();

                    Environment.Exit(0);
                    return;
                }
            }

            _server.FinishInit();
            GC.KeepAlive(_server);
        }

        [MTAThread]
        public static MangoServer GetServer()
        {
            return _server;
        }
    }
}
