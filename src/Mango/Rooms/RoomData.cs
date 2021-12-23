using System.Collections.Generic;
using Mango.Players;
using System.Text;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;

namespace Mango.Rooms
{
    class RoomData
    {
        private int _id;
        private int _ownerId;
        private string _name;
        private string _description;
        private List<string> _tags;
        private RoomType _type;
        private RoomAccess _access;
        private string _password;
        private int _categoryId;
        private int _maxUsers;
        private int _score;
        private RoomModel _model;
        private bool _allowPets;
        private bool _allowPetsEating;
        private bool _disableRoomBlocking;
        private bool _hideWalls;
        private int _wallThickness;
        private int _floorThickness;
        private Dictionary<string, string> _decorations;
        private List<int> _usersWithRights;
        private RoomPromotion _promotion;

        public RoomData(int Id, int OwnerId, string Name, string Description, string Tags, string Access, string Password,
            int CategoryId, int MaxUsers, int Score, RoomModel Model, int AllowPets, int AllowPetsEating, int DisableRoomBlocking,
            int HideWalls, int WallThickness, int FloorThickness, string Decorations)
        {
            this.Id = Id;
            this.OwnerId = OwnerId;
            this.Name = Name;
            this.Description = Description;

            List<string> TagsList = new List<string>();

            foreach (string s in Tags.Split(','))
            {
                TagsList.Add(s);
            }

            this.Tags = TagsList;

            this.Type = RoomType.FLAT;

            if (Access != "open" && Access != "doorbell" && Access != "password")
                throw new DatabaseException(string.Format("Expected data to be 'open' or 'doorbell' or 'password' but was '{0}'.", Access));

            switch (Access)
            {
                case "open":
                    this.Access = RoomAccess.Open;
                    break;

                case "doorbell":
                    this.Access = RoomAccess.Locked;
                    break;

                case "password":
                    this.Access = RoomAccess.Password_Protected;
                    break;
            }

            this.Password = Password;
            this.CategoryId = CategoryId;
            this.MaxUsers = MaxUsers;
            this.Score = Score;
            this.Model = Model;
            this.AllowPets = AllowPets == 1 ? true : false;
            this.AllowPetsEating = AllowPetsEating == 1 ? true : false;
            this.DisableRoomBlocking = DisableRoomBlocking == 1 ? true : false;
            this.HideWalls = HideWalls == 1 ? true : false;
            this.WallThickness = WallThickness;
            this.FloorThickness = FloorThickness;

            Dictionary<string, string> Dec = new Dictionary<string, string>();

            string[] DecorationBits = Decorations.Split('|');
            foreach (string d in DecorationBits)
            {
                string[] s = d.Split('=');

                if (s.Length == 2)
                {
                    Dec.Add(s[0], s[1]);
                }
            }

            this.Decorations = Dec;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int OwnerId
        {
            get { return this._ownerId; }
            set { this._ownerId = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        public List<string> Tags
        {
            get { return this._tags; }
            set { this._tags = value; }
        }

        public RoomType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public RoomAccess Access
        {
            get { return this._access; }
            set { this._access = value; }
        }

        public string Password
        {
            get { return this._password; }
            set { this._password = value; }
        }

        public int CategoryId
        {
            get { return this._categoryId; }
            set { this._categoryId = value; }
        }

        public int MaxUsers
        {
            get { return this._maxUsers; }
            set { this._maxUsers = value; }
        }

        public int Score
        {
            get { return this._score; }
            set { this._score = value; }
        }

        public RoomModel Model
        {
            get { return this._model; }
            set { this._model = value; }
        }

        public bool AllowPets
        {
            get { return this._allowPets; }
            set { this._allowPets = value; }
        }

        public bool AllowPetsEating
        {
            get { return this._allowPetsEating; }
            set { this._allowPetsEating = value; }
        }

        public bool DisableRoomBlocking
        {
            get { return this._disableRoomBlocking; }
            set { this._disableRoomBlocking = value; }
        }

        public bool HideWalls
        {
            get { return this._hideWalls; }
            set { this._hideWalls = value; }
        }

        public int WallThickness
        {
            get { return this._wallThickness; }
            set { this._wallThickness = value; }
        }

        public int FloorThickness
        {
            get { return this._floorThickness; }
            set { this._floorThickness = value; }
        }

        public Dictionary<string, string> Decorations
        {
            get { return this._decorations; }
            set { this._decorations = value; }
        }

        public List<int> UsersWithRights
        {
            get
            {
                if (this._usersWithRights == null)
                {
                    this._usersWithRights = new List<int>();

                    using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                    {
                        DbCon.SetQuery("SELECT * FROM `room_rights` WHERE `room_id` = @id;");
                        DbCon.AddParameter("id", this.Id);
                        DbCon.Open();

                        using (MySqlDataReader Reader = DbCon.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                this._usersWithRights.Add(Reader.GetInt32("user_id"));
                            }
                        }
                    }
                }

                return this._usersWithRights;
            }
            set { this._usersWithRights = value; }
        }

        public RoomPromotion Promotion
        {
            get { return this._promotion; }
            set { this._promotion = value; }
        }

        public RoomData(RoomData Data)
        {
            this.Id = Data.Id;
            this.OwnerId = Data.OwnerId;
            this.Name = Data.Name;
            this.Description = Data.Description;
            this.Tags = Data.Tags;
            this.Type = Data.Type;
            this.Access = Data.Access;
            this.Password = Data.Password;
            this.CategoryId = Data.CategoryId;
            this.MaxUsers = Data.MaxUsers;
            this.Score = Data.Score;
            this.Model = Data.Model;
            this.AllowPets = Data.AllowPets;
            this.AllowPetsEating = Data.AllowPetsEating;
            this.DisableRoomBlocking = Data.DisableRoomBlocking;
            this.HideWalls = Data.HideWalls;
            this.WallThickness = Data.WallThickness;
            this.FloorThickness = Data.FloorThickness;
            this.Decorations = Data.Decorations;
            this.UsersWithRights = Data.UsersWithRights;
            this.Promotion = Data.Promotion;
        }

        public virtual int UsersNow
        {
            get
            {
                return 0;
            }
        }

        public string OwnerName
        {
            get
            {
                return PlayerLoader.GetPlayerNameById(OwnerId);
            }
        }

        public virtual bool CanTrade
        {
            get
            {
                return false;
            }
        }

        public virtual bool OwnerInRoom
        {
            get
            {
                return false;
            }
        }
    }
}
