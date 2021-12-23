using System;
using System.Collections.Generic;
using log4net;
using Mango.Communication.Sessions;
using Mango.Players.Inventory;
using Mango.Players.Messenger;
using Mango.Rooms;
using Mango.Utilities;
using Mango.Players.Badges;
using Mango.Players.Achievements;
using Mango.Players.Wardrobe;
using Mango.Permissions;
using Mango.Players.Permissions;
using Mango.Players.Subscriptions;
using Mango.Players.Effects;
using Mango.Players.Process;
using Mango.Players.Favourites;
using Mango.Rooms.Avatar;

namespace Mango.Players
{
    class Player : PlayerData
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Players.Player");

        /// <summary>
        /// The session linked and used for this active player.
        /// </summary>
        private Session _session = null;

        /// <summary>
        /// Timed process to update pixels, credits etc
        /// </summary>
        private ProcessComponent _process = null;

        /// <summary>
        /// Permissions for this user.
        /// </summary>
        private PermissionComponent _permissions = null;

        /// <summary>
        /// Subscriptions for this user.
        /// </summary>
        private SubscriptionComponent _subscriptions = null;

        /// <summary>
        /// Wardrobe for this user.
        /// </summary>
        private WardrobeComponent _wardrobe = null;

        /// <summary>
        /// The inventory component used for this player.
        /// </summary>
        private InventoryComponent _inventory = null;

        /// <summary>
        /// The messenger component used for this player.
        /// </summary>
        private MessengerComponent _messenger = null;

        /// <summary>
        /// The badge component used for this player.
        /// </summary>
        private BadgeComponent _badges = null;

        /// <summary>
        /// The achievement componenet used for this player.
        /// </summary>
        private AchievementComponent _achievements = null;

        /// <summary>
        /// The Effects component for this Player.
        /// </summary>
        private EffectsComponent _effects = null;

        /// <summary>
        /// Favourite Rooms
        /// </summary>
        private FavouritesComponent _favouriteRooms = null;

        /// <summary>
        /// The RoomAvatar for this player used for rooms.
        /// </summary>
        private RoomAvatar _avatar = null;

        /// <summary>
        /// Mark the object as disposed.
        /// </summary>
        private Boolean _isDisposed = false;

        /// <summary>
        /// Initializes a new Player instance using the Info constructed from a previous PlayerInfo class.
        /// </summary>
        /// <param name="Data">PlayerInfo to use with this Instance.</param>
        public Player(Session Session, PlayerData Data)
            : base(Data)
        {
            this._session = Session;
            this._avatar = new RoomAvatar(this);

            log.Info("<Player " + this.Id + "> has joined the game.");
        }

        public bool IsDisposed
        {
            get { return this._isDisposed; }
        }

        public bool InitializeComponents()
        {
            return this.InitWardrobe() && this.InitFavourites()
                && this.InitMessenger() && this.InitInventory() && this.InitSubscription()
                && this.InitSubscription() && this.InitAchievements() && this.InitBadges()
                && this.InitEffects() && this.InitPermissions();
        }

        public void InitProcess()
        {
            this._process = new ProcessComponent();
            this._process.Init(this);

            log.Debug("<Player " + this.Id + "> Processing has initialized.");
        }

        private bool InitWardrobe()
        {
            this._wardrobe = new WardrobeComponent();

            if (this._wardrobe.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Wardrobe has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitPermissions()
        {
            this._permissions = new PermissionComponent();

            if (this._permissions.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Permission has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitSubscription()
        {
            this._subscriptions = new SubscriptionComponent();

            if (this._subscriptions.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Subscription has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitMessenger()
        {
            this._messenger = new MessengerComponent();

            if (this._messenger.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Messenger has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitInventory()
        {
            this._inventory = new InventoryComponent();

            if (this._inventory.Init(this.Id))
            {
                log.Debug("<Player " + this.Id + "> Inventory has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitBadges()
        {
            if (this._achievements == null)
            {
#if DEBUG
                throw new InvalidOperationException("Achievements must be initialized before badges.");
#endif
            }

            this._badges = new BadgeComponent();

            if (this._badges.Init(this, this._achievements))
            {
                log.Debug("<Player " + this.Id + "> Badges has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitAchievements()
        {
            this._achievements = new AchievementComponent();

            if (this._achievements.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Achievements has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitEffects()
        {
            this._effects = new EffectsComponent();

            if (this._effects.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Effects has been initialized.");
                return true;
            }

            return false;
        }

        private bool InitFavourites()
        {
            this._favouriteRooms = new FavouritesComponent();

            if (this._favouriteRooms.Init(this))
            {
                log.Debug("<Player " + this.Id + "> Room Favourites has been initialized.");
                return true;
            }

            return false;
        }

        public void UpdateCreditBalance(int Amount)
        {
            this.Credits += Amount;
            this.Save();
        }

        public void UpdatePixelBalance(int Amount)
        {
            this.Pixels += Amount;
            this.Save();
        }

        public void UpdateVolume(int Volume)
        {
            this.ClientVolume = Volume;
            this.Save();
        }

        public void UpdateFigure(string Figure)
        {
            this.Figure = Figure;
            this.Save();
        }

        public void IncreaseRespect()
        {
            base.RespectPoints++;
            this.Save();
        }

        public void DecreaseRespectToGivePlayer()
        {
            base.RespectPointsLeftPlayer--;
            this.Save();
        }

        public void UpdateGender(string Gender)
        {
            PlayerGender GenderEnum = PlayerGender.MALE;

            if (Gender.ToLower() == "f")
            {
                GenderEnum = PlayerGender.FEMALE;
            }

            this.Gender = GenderEnum;
            this.Save();
        }

        public void Mute(int TimeToMute)
        {
            this.ModMutedUntil = UnixTimestamp.GetNow() + TimeToMute;
            this.Save();
        }

        public void ChangeMotto(string Motto)
        {
            this.Motto = Motto;
            this.Save();
        }

        public void ChangeHomeRoom(int HomeRoom)
        {
            this.HomeRoom = HomeRoom;
            this.Save();
        }

        /// <summary>
        /// Performs any cleanup required when the player object is no longer needed.
        /// </summary>
        public void Cleanup()
        {
            if (this._isDisposed)
            {
                throw new InvalidOperationException("Cleanup method on player can only be called once.");
            }

            this._isDisposed = true;

            if (this._process != null)
            {
                this._process.Dispose();
            }

            if (!Mango.GetServer().GetPlayerManager().TryRemove(this))
            {
            }

            try
            {
                if (this._avatar.InRoom)
                {
                    this._avatar.ExitRoom(false); // to-do: improve sometimes null errors
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Exiting Room Error", ex);
            }

            if (this._messenger != null)
            {
                this._messenger.SetUpdateNeeded(true);
                this._messenger.Dispose();
            }

            if (this._inventory != null)
            {
                this._inventory.Dispose();
            }

            // This must be done to prevent the session from staying referenced here
            this._session = null;

            log.Info("<Player " + this.Id + "> has left the game.");
        }

        public int CurrentRoomId
        {
            get
            {
                return this._avatar.CurrentRoomId;
            }
        }

        public override bool InRoom
        {
            get
            {
                return this._avatar.InRoom;
            }
        }

        public override bool Online
        {
            get
            {
                return (!this._isDisposed);
            }
        }

        public override bool CanTrade
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// If the user is muted or has password the muted time.
        /// </summary>
        public bool Muted
        {
            get
            {
                return (this.ModMutedUntil - UnixTimestamp.GetNow()) > 0;
            }
        }

        public int MutedSecondsLeft
        {
            get
            {
                return Convert.ToInt32(Math.Round((this.ModMutedUntil - UnixTimestamp.GetNow())));
            }
        }

        public PermissionComponent GetPermissions()
        {
            return this._permissions;
        }

        public SubscriptionComponent GetSubscriptions()
        {
            return this._subscriptions;
        }

        public WardrobeComponent GetWardrobe()
        {
            return this._wardrobe;
        }

        public InventoryComponent GetInventory()
        {
            return this._inventory;
        }

        public MessengerComponent GetMessenger()
        {
            return this._messenger;
        }

        public BadgeComponent GetBadges()
        {
            return this._badges;
        }

        public AchievementComponent Achievements()
        {
            return this._achievements;
        }

        public EffectsComponent Effects()
        {
            return this._effects;
        }

        public FavouritesComponent Favourites()
        {
            return this._favouriteRooms;
        }

        public RoomAvatar GetAvatar()
        {
            return this._avatar;
        }

        public Session GetSession()
        {
            if (this._isDisposed)
            {
                throw new NullReferenceException("Cannot call back to session once the player is disposed.");
            }

            return this._session;
        }
    }
}
