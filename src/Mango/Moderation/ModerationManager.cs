using log4net;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Moderation
{
    sealed class ModerationManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Moderation.ModerationManager");

        private List<string> _userPresets = new List<string>();
        private List<string> _roomPresets = new List<string>();
        private Dictionary<int, string> _userActionPresetCategories = new Dictionary<int, string>();
        private Dictionary<int, Dictionary<string, string>> _userActionPresetMessages = new Dictionary<int, Dictionary<string, string>>();

        public void Init()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `moderation_presets`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            string Type = Reader.GetString("type").ToLower();

                            switch (Type)
                            {
                                case "user":
                                    this._userPresets.Add(Reader.GetString("message"));
                                    break;

                                case "room":
                                    this._roomPresets.Add(Reader.GetString("message"));
                                    break;
                            }
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load ModerationPreset for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `moderation_preset_action_categories`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._userActionPresetCategories.Add(Reader.GetInt32("id"), Reader.GetString("caption"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load ModerationActionCategory for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `moderation_preset_action_messages`;");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int ParentId = Reader.GetInt32("parent_id");

                            if (!this._userActionPresetMessages.ContainsKey(ParentId))
                            {
                                this._userActionPresetMessages.Add(ParentId, new Dictionary<string, string>());
                            }

                            this._userActionPresetMessages[ParentId].Add(Reader.GetString("caption"), Reader.GetString("message_text"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load ModerationActionMessage for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            log.Info("Loaded " + (this._userPresets.Count + this._roomPresets.Count) + " moderation presets.");
            log.Info("Loaded " + this._userActionPresetCategories.Count + " moderation categories.");
            log.Info("Loaded " + this._userActionPresetMessages.Count + " moderation action preset messages.");
        }

        public ICollection<string> UserMessagePresets
        {
            get { return this._userPresets; }
        }

        public ICollection<string> RoomMessagePresets
        {
            get { return this._roomPresets; }
        }

        public Dictionary<string, Dictionary<string, string>> UserActionPresets
        {
            get
            {
                Dictionary<string, Dictionary<string, string>> Result = new Dictionary<string, Dictionary<string, string>>();

                foreach (KeyValuePair<int, string> Category in this._userActionPresetCategories)
                {
                    Result.Add(Category.Value, new Dictionary<string, string>());

                    if (this._userActionPresetMessages.ContainsKey(Category.Key))
                    {
                        foreach (KeyValuePair<string, string> Data in this._userActionPresetMessages[Category.Key])
                        {
                            Result[Category.Value].Add(Data.Key, Data.Value);
                        }
                    }
                }

                return Result;
            }
        }
    }
}
