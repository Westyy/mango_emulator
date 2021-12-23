using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Mango.Communication.Sessions;
using Mango.Rooms.Instance;
using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Rooms.Avatar;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Room.Action;
using Mango.Items;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;
using MySql.Data.MySqlClient;
using Mango.Utilities;

namespace Mango.Rooms
{
    sealed class RoomInstance : RoomData
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Rooms.RoomInstance");

        private AvatarsComponent Avatars = null;
        private BansComponent Bans = null;
        private ItemsComponent Items = null;
        private RightsComponent Rights = null;
        private MappingComponent Mapping = null;
        private ProcessComponent Processing = null;

        public bool Unloaded { get; set; }
        public int IdleTime { get; set; }

        public RoomInstance(RoomData d)
            : base(d)
        {
            if (d == null)
            {
                throw new NullReferenceException("RoomData cannot be a null.");
            }

            this.Avatars = new AvatarsComponent(this);
            this.Bans = new BansComponent(this);
            this.Items = new ItemsComponent(this);
            this.Rights = new RightsComponent(this);
            this.Mapping = new MappingComponent(this);
            this.Processing = new ProcessComponent(this);

            this.Unloaded = false;
            this.IdleTime = 0;

            this.Mapping.RegenerateRelativeHeightmap();
            this.Processing.Init();
        }

        public override int UsersNow
        {
            get
            {
                return GetAvatars().Count;
            }
        }

        public override bool OwnerInRoom
        {
            get
            {
                return GetAvatars().Contains(base.OwnerId);
            }
        }

        public List<string> SearchableTags
        {
            get
            {
                return base.Tags;
            }
        }

        public bool HasActivePromotion
        {
            get
            {
                return base.Promotion != null;
            }
        }

        public void StartOrExtendPromotion()
        {
            if (this.HasActivePromotion)
            {
                this.Promotion.TimestampExpires += (120 * 60);
            }
            else
            {
                double TsNow = UnixTimestamp.GetNow();
                this.Promotion = new RoomPromotion(TsNow, TsNow);
            }
        }

        public void EndPromotion()
        {
            if (!this.HasActivePromotion) { throw new InvalidOperationException("Promotion not active"); }
            this.Promotion = null;
        }

        public void DeleteRoom(Session Session)
        {
            // Kick all users
            this.Avatars.RemoveAll();

            // to-do: this

            /*using (var s = Mango.GetServer().GetDatabaseOld().GetSessionFactory().OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    foreach (Item Item in this.GetItems().GetWallAndFloor)
                    {
                        if (Item.Data.Behaviour == ItemBehaviour.STICKY_NOTE)
                        {
                            s.Delete(Item);
                            continue;
                        }

                        Item.RoomId = 0;
                        Item.RoomWallPos = "";
                        Item.RoomRot = 0;
                        Item.X = 0;
                        Item.Y = 0;
                        Item.Z = 0;
                        Item.UserId = this.OwnerId;

                        s.SaveOrUpdate(Item);

                        Session.GetPlayer().GetInventory().TryAddItem(Item);
                    }

                    s.Delete("DbRoom", this);

                    tx.Commit();
                }
            }*/

            Session.SendPacket(new FurniListUpdateComposer());

            Mango.GetServer().GetRoomManager().UnloadRoom(this);
        }

        public bool UpdateDetails(string name, string description, RoomAccess access, string password, int userLimit,
            int categoryId, List<string> tags, bool allowPets, bool allowPetEating, bool disableBlocking, bool hideWalls,
            int wallThickness, int floorThickness)
        {
            base.Name = name;
            base.Description = description;
            base.Access = access;
            base.Password = password;
            base.MaxUsers = userLimit;
            base.CategoryId = categoryId;
            base.Tags = tags;
            base.AllowPets = allowPets;
            base.AllowPetsEating = AllowPetsEating;
            base.DisableRoomBlocking = disableBlocking;
            base.HideWalls = hideWalls;
            base.WallThickness = wallThickness;
            base.FloorThickness = floorThickness;

            string AccessStr = password.Length > 0 ? "password" : "open";

            switch (access)
            {
                case RoomAccess.Open:
                    AccessStr = "open";
                    break;

                case RoomAccess.Password_Protected:
                    AccessStr = "password";
                    break;

                case RoomAccess.Locked:
                    AccessStr = "doorbell";
                    break;
            }

            StringBuilder tagString = new StringBuilder();

            foreach (string s in tags)
            {
                tagString.Append(s);
                tagString.Append(",");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("UPDATE `rooms` SET `name` = @name, `description` = @desc, `access_type` = @access, `password` = @pass, `max_users` = @maxusers, `category` = @category, `tags` = @tags, `allow_pets` = @allowpets, `allow_pets_eating` = @allowpetseating, `disable_blocking` = @disableblock, `hide_walls` = @hidewalls, `thickness_wall` = @wallthick, `thickness_floor` = @floorthick WHERE `id` = @id;");
                    DbCon.AddParameter("name", name);
                    DbCon.AddParameter("desc", description);
                    DbCon.AddParameter("access", AccessStr);
                    DbCon.AddParameter("pass", password);
                    DbCon.AddParameter("maxusers", userLimit);
                    DbCon.AddParameter("category", categoryId);
                    DbCon.AddParameter("tags", tagString.ToString());
                    DbCon.AddParameter("allowpets", allowPets == true ? "1" : "0");
                    DbCon.AddParameter("allowpetseating", allowPetEating == true ? "1" : "0");
                    DbCon.AddParameter("disableblock", disableBlocking == true ? "1" : "0");
                    DbCon.AddParameter("hidewalls", hideWalls == true ? "1" : "0");
                    DbCon.AddParameter("wallthick", wallThickness);
                    DbCon.AddParameter("floorthick", floorThickness);
                    DbCon.AddParameter("id", base.Id);
                    DbCon.ExecuteNonQuery();

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                }
            }

            return true;
        }

        private object DecorationgApplySync = new object();

        public void ApplyDecoration(string Key, string Value)
        {
            lock (DecorationgApplySync)
            {
                if (!base.Decorations.ContainsKey(Key))
                {
                    base.Decorations.Add(Key, Value);
                }
                else
                {
                    base.Decorations[Key] = Value;
                }

                StringBuilder DecorationString = new StringBuilder();

                foreach (KeyValuePair<string, string> Decoration in base.Decorations)
                {
                    if (DecorationString.Length > 0)
                    {
                        DecorationString.Append('|');
                    }

                    DecorationString.Append(Decoration.Key);
                    DecorationString.Append('=');
                    DecorationString.Append(Decoration.Value);
                }

                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("UPDATE `rooms` SET `decorations` = @dec WHERE `id` = @id;");
                        DbCon.AddParameter("dec", DecorationString.ToString());
                        DbCon.AddParameter("id", base.Id);

                        DbCon.ExecuteNonQuery();
                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// Sends all room objects required for loading to the Session.
        /// </summary>
        /// <param name="Session"></param>
        public void SendObjects(Session Session)
        {
            Session.SendPacket(new HeightMapComposer(this.Model.Heightmap.ToString()));
            Session.SendPacket(new FloorHeightMapComposer(GetMapping().GetRelativeHeightmap()));

            List<RoomAvatar> Avatars = new List<RoomAvatar>();
            Dictionary<RoomAvatar, int> DancingAvatars = new Dictionary<RoomAvatar, int>();
            Dictionary<RoomAvatar, int> CarryAvatars = new Dictionary<RoomAvatar, int>();
            Dictionary<RoomAvatar, int> EffectAvatars = new Dictionary<RoomAvatar, int>();
            List<RoomAvatar> SleepingAvatars = new List<RoomAvatar>();

            foreach (RoomAvatar Avatar in GetAvatars().Avatars)
            {
                Avatars.Add(Avatar);

                if (Avatar.DanceId > 0)
                {
                    DancingAvatars.Add(Avatar, Avatar.DanceId);
                }

                if (Avatar.CarryItemId > 0)
                {
                    CarryAvatars.Add(Avatar, Avatar.CarryItemId);
                }

                if (Avatar.EffectId > 0)
                {
                    EffectAvatars.Add(Avatar, Avatar.EffectId);
                }

                if (Avatar.Sleeping)
                {
                    SleepingAvatars.Add(Avatar);
                }
            }

            Session.SendPacket(new UsersComposer(Avatars));
            //session.SendPacket(new PublicRoomObjectsComposer(new List<ItemStaticPublics>())); // need do statics for public rooms not that anyone uses them anymore
            Session.SendPacket(new ObjectsComposer(GetItems().GetFloor, this));
            Session.SendPacket(new ItemsComposer(GetItems().GetWall, this));
            Session.SendPacket(new UserUpdateComposer(Avatars));


            foreach (KeyValuePair<RoomAvatar, int> kvp in DancingAvatars)
            {
                Session.SendPacket(new DanceComposer(kvp.Key, kvp.Value));
            }

            foreach (KeyValuePair<RoomAvatar, int> kvp in CarryAvatars)
            {
                Session.SendPacket(new CarryObjectComposer(kvp.Key, kvp.Value));
            }

            foreach (KeyValuePair<RoomAvatar, int> kvp in EffectAvatars)
            {
                Session.SendPacket(new AvatarEffectComposer(kvp.Key, kvp.Value));
            }

            foreach (RoomAvatar avatar in SleepingAvatars)
            {
                Session.SendPacket(new SleepComposer(avatar, true));
            }
        }

        public void Unload()
        {
            if (this.Unloaded)
            {
                return;
            }

            this.Unloaded = true;

            this.Processing.Cleanup(); // stop processing first
            this.Avatars.RemoveAll(); // remove any remaining players from this room
            this.Avatars.Cleanup(); // cleanup avatars component
            this.Items.Cleanup();
            this.Mapping.Cleanup();
            this.Rights.Cleanup();

            this.Avatars = null;
            this.Bans = null;
            this.Items = null;
            this.Mapping = null;
            this.Processing = null;
            this.Rights = null;
        }

        public AvatarsComponent GetAvatars()
        {
            return this.Avatars;
        }

        public RightsComponent GetRights()
        {
            return this.Rights;
        }

        public BansComponent GetBans()
        {
            return this.Bans;
        }

        public ItemsComponent GetItems()
        {
            return this.Items;
        }

        public MappingComponent GetMapping()
        {
            return this.Mapping;
        }
    }
}
