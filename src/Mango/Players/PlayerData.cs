using System.Collections.Generic;
using System.Text;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;
using Mango.Rooms.Avatar;

namespace Mango.Players
{
    class PlayerData : IAvatarData
    {
        private int _id;
        private int _permissionLevel;
        private string _authTicket;
        private string _username;
        private string _alternativeName;
        private string _figure;
        private string _motto;
        private List<string> _tags;
        private PlayerGender _gender;
        private int _credits;
        private int _duckets;
        private int _homeRoom;
        private int _score;
        private bool _allowFriendRequests;
        private int _clientVolume;
   

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int PermissionLevel
        {
            get { return this._permissionLevel; }
            set { this._permissionLevel = value; }
        }

        public string AuthTicket
        {
            get { return this._authTicket; }
            set { this._authTicket = value; }
        }

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }

        public string AlternativeName
        {
            get { return this._alternativeName; }
            set { this._alternativeName = value; }
        }

        public string Figure
        {
            get { return this._figure; }
            set { this._figure = value; }
        }

        public string Motto
        {
            get { return this._motto; }
            set { this._motto = value; }
        }

        public List<string> Tags
        {
            get { return this._tags; }
            set { this._tags = value; }
        }

        public PlayerGender Gender
        {
            get { return this._gender; }
            set { this._gender = value; }
        }

        public int Credits
        {
            get { return this._credits; }
            set { this._credits = value; }
        }

        public int Duckets
        {
            get { return this._duckets; }
            set { this._duckets = value; }
        }

        public int HomeRoom
        {
            get { return this._homeRoom; }
            set { this._homeRoom = value; }
        }

        public int Score
        {
            get { return this._score; }
            set { this._score = value; }
        }

        public bool AllowFriendRequests
        {
            get { return this._allowFriendRequests; }
            set { this._allowFriendRequests = value; }
        }

        public int ClientVolume
        {
            get { return this._clientVolume; }
            set { this._clientVolume = value; }
        }

        

        public PlayerData(int Id, int PermissionLevel, string AuthTicket, string Username, string Figure,
            string Motto, string Gender, int Credits, int Duckets, int HomeRoom, int Score, int AllowFriendRequests, int ClientVolume)
        {
            this.Id = Id;
            this.PermissionLevel = PermissionLevel;
            this.AuthTicket = AuthTicket;
            this.Username = Username;
            this.Figure = Figure;
            this.Motto = Motto;

            if (Gender.ToLower() != "m" && Gender.ToLower() != "f")
                throw new DatabaseException("Gender is not 'm' or 'f'");

            switch (Gender.ToLower())
            {
                case "m":
                    this.Gender = PlayerGender.Male;
                    break;

                case "f":
                    this.Gender = PlayerGender.Female;
                    break;
            }

            this.Credits = Credits;
            this.Duckets = Duckets;
            this.HomeRoom = HomeRoom;
            this.Score = Score;
            this.AllowFriendRequests = AllowFriendRequests == 1 ? true : false;
            this.ClientVolume = ClientVolume;           
        }

        public PlayerData(PlayerData Data)
        {
            this.Id = Data.Id;
            this.PermissionLevel = Data.PermissionLevel;
            this.AuthTicket = Data.AuthTicket;
            this.Username = Data.Username;
            this.AlternativeName = Data.AlternativeName;
            this.Figure = Data.Figure;
            this.Motto = Data.Motto;
            this.Gender = Data.Gender;
            this.Credits = Data.Credits;
            this.Duckets = Data.Duckets;
            this.HomeRoom = Data.HomeRoom;
            this.Score = Data.Score;
            this.AllowFriendRequests = Data.AllowFriendRequests;
            this.ClientVolume = Data.ClientVolume;       
            this.Tags = Data.Tags;         
        }

        public virtual bool InRoom
        {
            get
            {
                return false;
            }
        }

        public virtual bool Online
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanTrade
        {
            get
            {
                return false;
            }
        }

        public bool Save()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                StringBuilder TagString = new StringBuilder();

                int c = 0;

                foreach (string Str in this.Tags)
                {
                    c++;

                    if (c > 6)
                    {
                        break;
                    }

                    TagString.Append(Str);
                    TagString.Append(",");
                }

                string Gender = this.Gender == PlayerGender.Male ? "M" : "F";

                DbCon.SetQuery("UPDATE `users` SET `username` = @username,  `figure` = @figure, `motto` = @motto, `tags` = @tags, `gender` = @gender, `credits` = @credits, `pixels` = @pixels, `home_room` = @homeroom, `score` = @score WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("username", this.Username);
                DbCon.AddParameter("realname", this.AlternativeName);
                DbCon.AddParameter("figure", this.Figure);
                DbCon.AddParameter("motto", this.Motto);
                DbCon.AddParameter("tags", TagString.ToString());
                DbCon.AddParameter("gender", Gender);
                DbCon.AddParameter("credits", this.Credits);
                DbCon.AddParameter("pixels", this.Duckets);
                DbCon.AddParameter("homeroom", this.HomeRoom);
                DbCon.AddParameter("score", this.Score);
                DbCon.AddParameter("acceptfr", this.AllowFriendRequests == true ? "1" : "0");
                DbCon.AddParameter("configvolume", this.ClientVolume);               
                DbCon.AddParameter("id", this.Id);

                DbCon.Open();
                DbCon.BeginTransaction();

                try
                {
                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                    return true;
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return false;
                }
            }
        }
    }
}
