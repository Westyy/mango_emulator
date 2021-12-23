using log4net;
using Mango.Rooms.Avatar;
using Mango.Utilities;
using MySql.Data.MySqlClient;
using System;

namespace Mango.Players
{
    class PlayerStats : IAvatarData
    {

        private static readonly ILog log = LogManager.GetLogger("Mango.Players.PlayerStats");

        private int _id;
        private string _username;
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
        private double _ducketsLastUpdated;


        public PlayerStats(int Id, string Username, int RespectPoints, int RespectPointsLeftPlayer, int RespectPointsLeftPet, 
            int ModTickets, int ModTicketsAbusive, double ModTicketsCoolDown, int ModBans, int ModCautions
            , double ModMutedUntil, double TimestampLastOnline, double TimestampRegistered, double DucketsLastUpdate)
        {
            this._id = Id;   
            this._username = Username;   
            this._respectPoints = RespectPoints;
            this._respectPointsLeftPlayer = RespectPointsLeftPlayer;
            this._respectPointsLeftPet = RespectPointsLeftPet;
            this._modTickets = ModTickets;
            this._modTicketsAbusive = ModTicketsAbusive;
            this._modTicketsCooldown = ModTicketsCooldown;
            this._modBans = ModBans;
            this._modCautions = ModCautions;
            this._modMutedUntil = ModMutedUntil;
            this._timestampLastOnline = TimestampLastOnline;
            this._timestampRegistered = TimestampRegistered;
            this._ducketsLastUpdated = DucketsLastUpdate;
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
        public void Mute(int TimeToMute)
        {
            this.ModMutedUntil = UnixTimestamp.GetNow() + TimeToMute;
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

        public double DucketsLastUpdated
        {
            get { return this._ducketsLastUpdated; }
            set { this._ducketsLastUpdated = value; }
        }
        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }
        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public void IncreaseRespect()
        {
            this.RespectPoints++;
        }

        public void DecreaseRespectToGivePlayer()
        {
            this.RespectPointsLeftPlayer--;          
        }

        public bool LoadPlayerStats(PlayerData data)
        {
            PlayerStats stats = null;
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `users_stats` WHERE `user_id` = @id LIMIT 1;");
                DbCon.AddParameter("id", data.Id);
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            stats = new PlayerStats(Reader.GetInt32("id"), Reader.GetString("username"), Reader.GetInt32("respects"),
                                Reader.GetInt32("respects_left_player"), Reader.GetInt32("respects_left_bot"), Reader.GetInt32("moderation_tickets"),
                                Reader.GetInt32("moderation_tickets_abusive"), Reader.GetDouble("moderation_tickets_cooldown"), Reader.GetInt32("moderation_bans"),
                                Reader.GetInt32("moderation_cautions"), Reader.GetDouble("moderation_muted_until"), Reader.GetDouble("timestamp_last_online"),
                                Reader.GetDouble("timestamp_registered"), Reader.GetDouble("duckets_last_updated"));
                                data.PlayerStats = stats;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Cannot load PlayerStats", ex);
                            stats = null;
                            return false;
                        }
                    }
                }
            }
            if (stats == null)
            {
                log.Error("Couldn't initialize Players Stats.");
                return false;
            }
            return false;
        }
    }
}
