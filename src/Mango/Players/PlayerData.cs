using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Rooms;
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
        private int _pixels;
        private int _homeRoom;
        private int _score;
        private bool _allowFriendRequests;
        private int _clientVolume;
        private int _respectPoints;
        private int _respectPointsLeftPlayer;
        private int _respectPointsLeftPet;
        private int _modTickets;
        private int _modTicketsAbusive;
        private double _modTicketsCooldown;
        private int _modBans;
        private int _modCautions;
        private double _modMutedUntil;
        private double _timestampLastOnline;
        private double _timestampRegistered;
        private double _pixelsLastUpdated;

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

        public int Pixels
        {
            get { return this._pixels; }
            set { this._pixels = value; }
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

        public int RespectPoints
        {
            get { return this._respectPoints; }
            set { this._respectPoints = value; }
        }

        public int RespectPointsLeftPlayer
        {
            get { return this._respectPointsLeftPlayer; }
            set { this._respectPointsLeftPlayer = value; }
        }

        public int RespectPointsLeftPet
        {
            get { return this._respectPointsLeftPet; }
            set { this._respectPointsLeftPet = value; }
        }

        public int ModTickets
        {
            get { return this._modTickets; }
            set { this._modTickets = value; }
        }

        public int ModTicketsAbusive
        {
            get { return this._modTicketsAbusive; }
            set { this._modTicketsAbusive = value; }
        }

        public double ModTicketsCooldown
        {
            get { return this._modTicketsCooldown; }
            set { this._modTicketsCooldown = value; }
        }

        public int ModBans
        {
            get { return this._modBans; }
            set { this._modBans = value; }
        }

        public int ModCautions
        {
            get { return this._modCautions; }
            set { this._modCautions = value; }
        }

        public double ModMutedUntil
        {
            get { return this._modMutedUntil; }
            set { this._modMutedUntil = value; }
        }

        public double TimestampLastOnline
        {
            get { return this._timestampLastOnline; }
            set { this._timestampLastOnline = value; }
        }

        public double TimestampRegistered
        {
            get { return this._timestampRegistered; }
            set { this._timestampRegistered = value; }
        }

        public double PixelsLastUpdated
        {
            get { return this._pixelsLastUpdated; }
            set { this._pixelsLastUpdated = value; }
        }

        public PlayerData(int Id, int PermissionLevel, string AuthTicket, string Username, string AlternativeName, string Figure,
            string Motto, string Gender, int Credits, int Pixels, int HomeRoom, int Score, int AllowFriendRequests, int ClientVolume,
            int RespectPoints, int RespectPointsLeftPlayer, int RespectPointsLeftPet, string Tags, int ModTickets, int ModTicketsAbusive,
            double ModTicketsCooldown, int ModBans, int ModCautions, double ModMutedUntil, double TimestampLastOnline, double TimestampRegistered,
            double PixelsLastUpdated)
        {
            this.Id = Id;
            this.PermissionLevel = PermissionLevel;
            this.AuthTicket = AuthTicket;
            this.Username = Username;
            this.AlternativeName = AlternativeName;
            this.Figure = Figure;
            this.Motto = Motto;

            if (Gender.ToLower() != "m" && Gender.ToLower() != "f")
                throw new DatabaseException("Gender is not 'm' or 'f'");

            switch (Gender.ToLower())
            {
                case "m":
                    this.Gender = PlayerGender.MALE;
                    break;

                case "f":
                    this.Gender = PlayerGender.FEMALE;
                    break;
            }

            this.Credits = Credits;
            this.Pixels = Pixels;
            this.HomeRoom = HomeRoom;
            this.Score = Score;
            this.AllowFriendRequests = AllowFriendRequests == 1 ? true : false;
            this.ClientVolume = ClientVolume;
            this.RespectPoints = RespectPoints;
            this.RespectPointsLeftPlayer = RespectPointsLeftPlayer;
            this.RespectPointsLeftPet = RespectPointsLeftPet;

            List<string> TagsList = new List<string>();

            foreach (string Str in Tags.Split(','))
            {
                TagsList.Add(Str);
            }

            this.Tags = TagsList;
            this.ModTickets = ModTickets;
            this.ModTicketsAbusive = ModTicketsAbusive;
            this.ModTicketsCooldown = ModTicketsCooldown;
            this.ModBans = ModBans;
            this.ModCautions = ModCautions;
            this.ModMutedUntil = ModMutedUntil;
            this.TimestampLastOnline = TimestampLastOnline;
            this.TimestampRegistered = TimestampRegistered;
            this.PixelsLastUpdated = this.PixelsLastUpdated;
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
            this.Pixels = Data.Pixels;
            this.HomeRoom = Data.HomeRoom;
            this.Score = Data.Score;
            this.AllowFriendRequests = Data.AllowFriendRequests;
            this.ClientVolume = Data.ClientVolume;
            this.RespectPoints = Data.RespectPoints;
            this.RespectPointsLeftPlayer = Data.RespectPointsLeftPlayer;
            this.RespectPointsLeftPet = Data.RespectPointsLeftPet;
            this.Tags = Data.Tags;
            this.ModTickets = Data.ModTickets;
            this.ModTicketsAbusive = Data.ModTicketsAbusive;
            this.ModTicketsCooldown = Data.ModTicketsCooldown;
            this.ModBans = Data.ModBans;
            this.ModCautions = Data.ModCautions;
            this.ModMutedUntil = Data.ModMutedUntil;
            this.TimestampLastOnline = Data.TimestampLastOnline;
            this.TimestampRegistered = Data.TimestampRegistered;
            this.PixelsLastUpdated = Data.PixelsLastUpdated;
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

                string Gender = this.Gender == PlayerGender.MALE ? "M" : "F";

                DbCon.SetQuery("UPDATE `users` SET `username` = @username, `real_name` = @realname, `figure` = @figure, `motto` = @motto, `tags` = @tags, `gender` = @gender, `credits` = @credits, `pixels` = @pixels, `home_room` = @homeroom, `score` = @score, `accept_friend_requests` = @acceptfr, `config_volume` = @configvolume, `respect_points` = @respect, `respect_left_player` = @respectplayer, `respect_left_pet` = @respectpet, `mod_tickets` = @modtickets, `mod_tickets_abusive` = @modticketsabusive, `mod_tickets_cooldown` = @modticketscooldown, `mod_bans` = @modbans, `mod_cautions` = @modcautions, `mod_muted_until_timestamp` = @modmuted, `timestamp_lastvisit` = @lastvisit, `pixels_last_updated` = @pixelsupdated WHERE `id` = @id LIMIT 1;");
                DbCon.AddParameter("username", this.Username);
                DbCon.AddParameter("realname", this.AlternativeName);
                DbCon.AddParameter("figure", this.Figure);
                DbCon.AddParameter("motto", this.Motto);
                DbCon.AddParameter("tags", TagString.ToString());
                DbCon.AddParameter("gender", Gender);
                DbCon.AddParameter("credits", this.Credits);
                DbCon.AddParameter("pixels", this.Pixels);
                DbCon.AddParameter("homeroom", this.HomeRoom);
                DbCon.AddParameter("score", this.Score);
                DbCon.AddParameter("acceptfr", this.AllowFriendRequests == true ? "1" : "0");
                DbCon.AddParameter("configvolume", this.ClientVolume);
                DbCon.AddParameter("respect", this.RespectPoints);
                DbCon.AddParameter("respectplayer", this.RespectPointsLeftPlayer);
                DbCon.AddParameter("respectpet", this.RespectPointsLeftPet);
                DbCon.AddParameter("modtickets", this.ModTickets);
                DbCon.AddParameter("modticketsabusive", this.ModTicketsAbusive);
                DbCon.AddParameter("modticketscooldown", this.ModTicketsCooldown);
                DbCon.AddParameter("modbans", this.ModBans);
                DbCon.AddParameter("modcautions", this.ModCautions);
                DbCon.AddParameter("modmuted", this.ModMutedUntil);
                DbCon.AddParameter("lastvisit", this.TimestampLastOnline);
                DbCon.AddParameter("pixelsupdated", this.PixelsLastUpdated);
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
