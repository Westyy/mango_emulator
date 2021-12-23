using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Nini.Config;
using Nini.Ini;
using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
namespace Mango
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Program");

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Entry point for the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            Console.Title = (Debugger.IsAttached ? "{DEBUG} Mango Server" : "Mango Server") + " " + Mango.Version;

            if (!File.Exists(@"Mango.exe.config"))
            {
                Console.WriteLine("Unable to run, missing configuration that cannot be re-created.");
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            string LogFile = (@"logs\MangoServer.log");

            if (Debugger.IsAttached && File.Exists(LogFile))
            {
                Console.WriteLine("!!!! Debugger Attached !!!! <" + LogFile + " cleared>");
                Console.WriteLine();
                File.Delete(LogFile);
            }

            // Configure XML
            XmlConfigurator.Configure();

            log.Info("Mango Server is starting");

            if (log.IsDebugEnabled)
            {
                log.Warn("Debugging - Log files will become large very quickly.");
                log.Warn("Press any key to continue...");
                Console.ReadKey();
            }

            if (!GCSettings.IsServerGC)
            {
                log.Warn("GC is not configured to server mode.");
            }

            log.Debug("GC latency mode is set to " + GCSettings.LatencyMode + " with GC Server set to " + (GCSettings.IsServerGC ? "Enabled" : "Disabled"));

            var sw = new Stopwatch();
            sw.Start();

            bool Running64Bit = (IntPtr.Size == 8);

            if (!Running64Bit)
            {
                log.Warn("This application is not running in 64-bit, we recommend you run it in 64-bit. Press any key to continue..");
                Console.ReadKey();
            }

            log.Info("Loading configuration file 'mango.conf'");

            if (!File.Exists(@"mango.conf"))
            {
                log.Error("Configuration file was not found, please make sure it exists and has the correct properties. Press any key to quit.");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            IConfigSource CfgSrc = new IniConfigSource(@"mango.conf");
            IConfig TcpCfg = CfgSrc.Configs["Network"];
            IConfig MysqlCfg = CfgSrc.Configs["MySql"];

            log.Info("Configuration file loaded successfully");

            MangoConfiguration Cfg = new MangoConfiguration
            {
                ServerIPAddress = TcpCfg.GetString("server_ip"),
                ServerPort = TcpCfg.GetInt("server_port"),
                ServerMaxConnections = TcpCfg.GetInt("max_connections"),
                MySqlIPAddress = MysqlCfg.GetString("mysql_ip"),
                MySqlPort = (uint)MysqlCfg.GetInt("mysql_port"),
                MySqlUser = MysqlCfg.GetString("mysql_user"),
                MySqlPassword = MysqlCfg.GetString("mysql_password"),
                MySqlDatabase = MysqlCfg.GetString("mysql_database"),
                MySqlMaxConnections = (uint)MysqlCfg.GetInt("mysql_maxconnections")
            };

            log.Info("Starting to initialize Mango Server");

            Mango BaseMango = new Mango(Cfg, args);
            GC.KeepAlive(BaseMango);

            sw.Stop();
            log.Debug("Time taken to start: " + sw.Elapsed.TotalSeconds.ToString().Split('.')[0] + " seconds.");

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("$mango> ");
                    string Input = Console.ReadLine();

                    if (Input.Length > 0)
                    {
                        string s = Input.Split(' ')[0]; // only the first bit (ignore after spaces)
                        ProgramConsoleCmds.Parse(s);
                    }
                }
            }
        }

        #region Fatal Exception Handling (Do not modify)

        private static Object _exceptionLock = new Object();
        private static Boolean _handlingException = false;

        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            // Only throw one exception, unhandled exceptions are fatal errors that should never happen.
            // This will prevent an army of exceptions been thrown.
            lock (_exceptionLock)
            {
                if (_handlingException)
                {
                    return;
                }

                _handlingException = true;
            }

            Exception ex = (Exception)args.ExceptionObject;
            
            // Throw the exception so the attached debugger can see it.
            if (Mango.IsDebugging)
            {
                throw ex;
            }

            Mango.GetServer().GetPacketManager().UnregisterAll();

            log.Fatal("Unhandled Error: " + ex.Message + " - " + ex.StackTrace);
            MessageBox.Show("Fatal error has occured, the server has been halted. Press OK to quit and open the crash log.", "Error", MessageBoxButtons.OK);

            if (File.Exists(@"logs\errors\MangoServer_ErrorLog.log"))
            {
                System.Diagnostics.Process.Start(@"logs\errors\MangoServer_ErrorLog.log");
            }

            Environment.Exit(0);

            // to-do: proper shutdown handling
        }
        #endregion
    }
}
