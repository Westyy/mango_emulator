using System;
using System.Net;
using log4net;
using Mango.Catalog;
using Mango.Communication;
using Mango.Communication.Packets;
using Mango.Database;
using Mango.Items;
using Mango.Navigator;
using Mango.Players;
using Mango.Rooms;
using MySql.Data.MySqlClient;
using Mango.Badges;
using Mango.Achievements;
using Mango.Quests;
using System.Threading;
using Mango.Permissions;
using Mango.Subscriptions;
using Mango.Chat;
using Mango.Items.Events;
using Mango.Moderation;
using Mango.Communication.Encryption;
using Mango.Utilities;
using Mango.Global;

namespace Mango
{
    sealed class MangoServer
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.MangoServer");

        /// <summary>
        /// Server configuration for connections & database.
        /// </summary>
        private readonly MangoConfiguration _globalConfiguration;

        /// <summary>
        /// Server database factory for handling entities and running queries.
        /// </summary>
        private DatabaseManager _database;

        /// <summary>
        /// ServerSocket handles accepting new connections, incoming and outgoing data.
        /// </summary>
        private ServerSocket _serverSocket;

        /// <summary>
        /// Stores globally registered packets
        /// </summary>
        private PacketManager _packetManager;

        /// <summary>
        /// PlayerManager stores players currently online.
        /// </summary>
        private PlayerManager _playerManager;

        private BadgeManager BadgeManager;
        private AchievementManager AchievementManager;
        private QuestManager QuestManager;
        private CatalogManager CatalogManager;
        private ItemDataManager ItemDataManager;
        private ItemEventManager ItemEventManager;
        private NavigatorManager NavigatorManager;
        private RoomManager RoomManager;
        private SubscriptionManager SubscriptionManager;
        private PermissionManager PermissionManager;
        private ModerationManager ModerationManager;
        private ChatManager ChatManager;

        private ServerStatusUpdater _globalUpdater;

        private HabboCrypto _crypto = null;

        /// <summary>
        /// Initializes a new instance of the MangoServer with the provided configuration settings.
        /// </summary>
        /// <param name="Config">Configuration settings.</param>
        public MangoServer(MangoConfiguration Config)
        {
            this._globalConfiguration = Config;

            ServerSocketSettings Settings = new ServerSocketSettings
            {
                MaxConnections = Config.ServerMaxConnections,
                NumOfSaeaForRec = Config.ServerMaxConnections,
                Backlog = 30,
                MaxSimultaneousAcceptOps = 15,
                BufferSize = 512,
                Endpoint = new IPEndPoint(IPAddress.Parse(Config.ServerIPAddress), Config.ServerPort)
            };

            this._serverSocket = new ServerSocket(Settings);
        }

        public void Initialize()
        {
            MySqlConnectionStringBuilder cs = new MySqlConnectionStringBuilder
            {
                ConnectionLifeTime = (60 * 5),
                ConnectionTimeout = 30,
                Database = this._globalConfiguration.MySqlDatabase,
                DefaultCommandTimeout = 120,
                Logging = false,
                MaximumPoolSize = this._globalConfiguration.MySqlMaxConnections,
                MinimumPoolSize = 3,
                Password = this._globalConfiguration.MySqlPassword,
                Pooling = true,
                Port = this._globalConfiguration.MySqlPort,
                Server = this._globalConfiguration.MySqlIPAddress,
                UseCompression = false,
                UserID = this._globalConfiguration.MySqlUser,
            };

            Console.WriteLine();

            this._database = new DatabaseManager(cs.ToString());

            if (!this._database.TestConnection())
            {
                log.Error("Unable to connect to database, check settings and restart Mango. Press any key to quit.");
                Console.ReadKey();

                Environment.Exit(0);
            }

            this._crypto = new HabboCrypto(HabboCryptoKeys.n, HabboCryptoKeys.e, HabboCryptoKeys.d);

            this._packetManager = new PacketManager();
            this._packetManager.RegisterGames();
            this._packetManager.RegisterCatalog();
            this._packetManager.RegisterHandshake();
            this._packetManager.RegisterInventoryAchievements();
            this._packetManager.RegisterInventoryAvatarEffects();
            this._packetManager.RegisterInventoryBadges();
            this._packetManager.RegisterInventoryFurni();
            this._packetManager.RegisterInventoryPurse();
            this._packetManager.RegisterMessenger();
            this._packetManager.RegisterModerator();
            this._packetManager.RegisterNavigator();
            this._packetManager.RegisterQuest();
            this._packetManager.RegisterRegister();
            this._packetManager.RegisterAdvertisement();
            this._packetManager.RegisterAvatar();
            this._packetManager.RegisterRoomAvatar();
            this._packetManager.RegisterRoomChat();
            this._packetManager.RegisterRoomAction();
            this._packetManager.RegisterRoom();
            this._packetManager.RegisterRoomFurniture();
            this._packetManager.RegisterSound();
            this._packetManager.RegisterTracking();
            this._packetManager.RegisterUsers();

            this._playerManager = new PlayerManager(32, this._globalConfiguration.ServerMaxConnections);

            this.RoomManager = new RoomManager();
            this.RoomManager.Init();

            this.ItemDataManager = new ItemDataManager();
            this.ItemDataManager.Init();

            this.ItemEventManager = new ItemEventManager();

            this.CatalogManager = new CatalogManager();
            this.CatalogManager.Init(this.ItemDataManager);

            this.NavigatorManager = new NavigatorManager();
            this.NavigatorManager.Init();

            this.BadgeManager = new BadgeManager();
            this.BadgeManager.Init();

            this.AchievementManager = new AchievementManager();
            this.AchievementManager.Init();

            this.QuestManager = new QuestManager();
            this.QuestManager.Init();

            this.SubscriptionManager = new SubscriptionManager();
            this.SubscriptionManager.Init();

            this.PermissionManager = new PermissionManager();
            this.PermissionManager.Init();

            this.ChatManager = new ChatManager();

            this.ModerationManager = new ModerationManager();
            this.ModerationManager.Init();

            this._serverSocket.Init();

            Console.WriteLine();

            log.Info("Mango Server has been initialized");
        }

        /// <summary>
        /// Brings the server online so players can begin to connect.
        /// </summary>
        public void FinishInit()
        {
            this._serverSocket.StartListen();

            log.Info("Server is now online");

            this._globalUpdater = new ServerStatusUpdater();
            this._globalUpdater.Init();
        }

        /*
         * Shutdown
         * 
         * 1 > Unregister all packets & wait for pending packets to finish processing.
         * 2 > Wait for all pending commands to finish processing (those which run async, ones which don't come under the 'packet processing').
         * 3 > Wait for all pending item events to finish executing.
         * 4 > Unload all users and save there data to the database including inventory.
         * 5 > Unload all rooms and save all items to the database.
         * 6 > Shutdown database and clear the connections to the database.
         * 7 > Exit the program
         * 
         * */

        /// <summary>
        /// Starts the safe shutdown sequence for the MangoServer.
        /// </summary>
        public void Shutdown()
        {
            // disable all logging
            LogManager.GetRepository().Threshold = LogManager.GetRepository().LevelMap["OFF"];

            Console.Clear();

            Console.WriteLine();
            Console.WriteLine("Server is shutting down, do not terminate Mango Server.");

            // unregister all packets
            Console.WriteLine("Unregistering all packets.");
            this._packetManager.UnregisterAll();

            // main server updater
            Console.WriteLine("Shutting down global updater.");
            this._globalUpdater.Dispose();

            // rooms
            Console.WriteLine("Disposing rooms and room manager.");
            this.RoomManager.Dispose();

            Console.WriteLine("Waiting for all packets to finish executing... (may take up to 5 minutes)");

            GetPacketManager().WaitForAllToComplete();

            Console.WriteLine("Disconnecting all players...");

            GetPlayerManager().DisconnectAll();

            Console.WriteLine("Closing the database connections...");

            // to-do: the shutting down

            Console.WriteLine("All done... Press any key to exit.");

            Console.ReadKey();

            Environment.Exit(0);
        }

        public MangoConfiguration GetConfig()
        {
            return this._globalConfiguration;
        }

        public DatabaseManager GetDatabase()
        {
            return this._database;
        }

        public PacketManager GetPacketManager()
        {
            return this._packetManager;
        }

        public PlayerManager GetPlayerManager()
        {
            return this._playerManager;
        }

        public CatalogManager GetCatalogManager()
        {
            return this.CatalogManager;
        }

        public ItemDataManager GetItemDataManager()
        {
            return this.ItemDataManager;
        }

        public ItemEventManager GetItemEventManager()
        {
            return this.ItemEventManager;
        }

        public NavigatorManager GetNavigatorManager()
        {
            return this.NavigatorManager;
        }

        public RoomManager GetRoomManager()
        {
            return this.RoomManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return this.BadgeManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return this.AchievementManager;
        }

        public QuestManager GetQuestManager()
        {
            return this.QuestManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return this.SubscriptionManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return this.PermissionManager;
        }

        public ChatManager GetChatManager()
        {
            return this.ChatManager;
        }

        public ModerationManager GetModerationManager()
        {
            return this.ModerationManager;
        }

        public HabboCrypto GetCrypto()
        {
            return this._crypto;
        }
    }
}
