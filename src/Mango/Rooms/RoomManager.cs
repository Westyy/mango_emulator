using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Mango.Subscriptions;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;
using Mango.Items;

namespace Mango.Rooms
{
    sealed class RoomManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Rooms.RoomManager");

        private readonly ConcurrentDictionary<int, RoomInstance> _rooms;
        private readonly Dictionary<string, RoomModel> _roomModels;

        private readonly Object _roomLoadingSync;

        [Obsolete("Replaced by the RoomLoadingSync.")]
        private bool _disposed = false;

        public RoomManager()
        {
            this._rooms = new ConcurrentDictionary<int, RoomInstance>();
            this._roomModels = new Dictionary<string, RoomModel>();
            this._roomLoadingSync = new Object();
        }

        public void Init()
        {
            LoadRoomModels();
        }

        /// <summary>
        /// Initializes and loads Room Models.
        /// </summary>
        private void LoadRoomModels()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `room_models` WHERE `enabled` = '1'");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this._roomModels.Add(Reader.GetString("id"), new RoomModel(Reader.GetString("id"), Reader.GetString("type"),
                                new Heightmap(Reader.GetString("heightmap")), Reader.GetInt32("door_x"), Reader.GetInt32("door_y"),
                                Reader.GetDouble("door_z"), Reader.GetInt32("door_dir"), Reader.GetInt32("max_users"), Reader.GetString("subscription_requirement")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load RoomModel for ID[" + Reader.GetString("id") + "]", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to load a room with the given id.
        /// </summary>
        /// <param name="roomId">Room id to load.</param>
        /// <param name="instance">The room instance loaded</param>
        /// <returns>If it succeeded or not.</returns>
        public bool TryLoadRoom(int roomId, out RoomInstance instance)
        {
            RoomInstance inst = null;

            if (this._rooms.TryGetValue(roomId, out inst))
            {
                if (!inst.Unloaded)
                {
                    instance = inst;
                    return true;
                }
                {
                    instance = null;
                    return false;
                }
            }

            lock (this._roomLoadingSync) // ensure rooms are only loaded once
            {
                /*if (this._disposed)
                {
                    instance = null;
                    return false;
                }*/

                if (this._rooms.TryGetValue(roomId, out inst))
                {
                    if (!inst.Unloaded)
                    {
                        instance = inst;
                        return true;
                    }
                    {
                        instance = null;
                        return false;
                    }
                }

                RoomData data = null;

                if (!RoomLoader.TryGetData(roomId, out data))
                {
                    instance = null;
                    return false;
                }

                RoomInstance myInstance = new RoomInstance(data);

                if (this._rooms.TryAdd(roomId, myInstance))
                {
                    log.Info("<Room " + roomId + " expected " + data.Id + "> was loaded successfully.");
                    instance = myInstance;
                    return true;
                }
                else
                {
                    log.Info("<Room " + roomId + " expected " + data.Id + "> failed to load (a room with the same key already exists)");
                    instance = null;
                    return false;
                }
            }
        }

        public void UnloadRoom(RoomInstance instance)
        {
            RoomInstance inst = null;

            if (this._rooms.TryRemove(instance.Id, out inst))
            {
                inst.Unload();

                log.Info("<Room " + inst.Id + "> has been unloaded.");
            }
            else
            {
                throw new InvalidOperationException("Failed to unload room when the room should be loaded.");
            }
        }

        public List<RoomData> SearchRooms(string Query, bool ByOwnerId = false, int OwnerId = 0, int SearchEventCatId = 0)
        {
            if (ByOwnerId && OwnerId > 0)
            {
                IEnumerable<RoomData> InstanceMatches =
                    (from RoomInstance in this._rooms
                     where RoomInstance.Value.UsersNow > 0 &&
                     RoomInstance.Value.Type == RoomType.FLAT &&
                     RoomInstance.Value.OwnerId == OwnerId
                     orderby RoomInstance.Value.UsersNow descending
                     select RoomInstance.Value).Take(50);

                return InstanceMatches.ToList();
            }
            else
            {
                IEnumerable<RoomData> InstanceMatches =
                        (from RoomInstance in this._rooms
                         where RoomInstance.Value.UsersNow > 0 &&
                             RoomInstance.Value.Type == RoomType.FLAT &&
                             (RoomInstance.Value.OwnerName.StartsWith(Query) ||
                             RoomInstance.Value.SearchableTags.Contains(Query) ||
                             RoomInstance.Value.Name.Contains(Query))
                         orderby RoomInstance.Value.UsersNow descending
                         select RoomInstance.Value).Take(50);

                return InstanceMatches.ToList();
            }
        }

        public List<RoomData> GetPopularRooms(int category)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.Type == RoomType.FLAT &&
                    RoomInstance.Value.UsersNow > 0 &&
                    (category == -1 || RoomInstance.Value.CategoryId == category)
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(50);

            return rooms.ToList();
        }

        public List<RoomData> GetPopularRatedRooms()
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.Type == RoomType.FLAT &&
                    RoomInstance.Value.UsersNow > 0
                 orderby RoomInstance.Value.Score descending
                 select RoomInstance.Value).Take(50);

            return rooms.ToList();
        }

        public List<RoomData> GetOnGoingRoomPromotions(int Mode)
        {
            IEnumerable<RoomData> Rooms = null;

            if (Mode == 17)
            {
                Rooms =
                    (from RoomInstance in this._rooms
                     where (RoomInstance.Value.HasActivePromotion)
                     orderby RoomInstance.Value.Promotion.TimestampStarted descending
                     select RoomInstance.Value).Take(50);
            }
            else
            {
                Rooms =
                    (from RoomInstance in this._rooms
                     where (RoomInstance.Value.HasActivePromotion)
                     orderby RoomInstance.Value.UsersNow descending
                     select RoomInstance.Value).Take(50);
            }

            return Rooms.ToList();
        }

        public List<KeyValuePair<string, int>> GetPopularRoomTags()
        {
            IEnumerable<List<string>> Tags =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.UsersNow > 0
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value.SearchableTags).Take(50);

            Dictionary<string, int> TagValues = new Dictionary<string, int>();

            foreach (List<string> TagList in Tags)
            {
                foreach (string Tag in TagList)
                {
                    if (!TagValues.ContainsKey(Tag))
                    {
                        TagValues.Add(Tag, 1);
                    }
                    else
                    {
                        TagValues[Tag]++;
                    }
                }
            }

            List<KeyValuePair<string, int>> SortedTags = new List<KeyValuePair<string, int>>(TagValues);

            SortedTags.Sort((FirstPair, NextPair) =>
            {
                return FirstPair.Value.CompareTo(NextPair.Value);
            });

            SortedTags.Reverse();

            return SortedTags;
        }

        public RoomInstance TryGetRandomLoadedRoom()
        {
            IEnumerable<RoomInstance> room =
                (from RoomInstance in this._rooms
                 where (RoomInstance.Value.UsersNow > 0 &&
                    RoomInstance.Value.Access == RoomAccess.Open &&
                    RoomInstance.Value.UsersNow < RoomInstance.Value.MaxUsers &&
                    RoomInstance.Value.Type == RoomType.FLAT)
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(1);

            if (room.Count() > 0)
            {
                return room.First();
            }
            else
            {
                return null;
            }
        }

        public bool TryGetRoom(int roomId, out RoomInstance instance)
        {
            return this._rooms.TryGetValue(roomId, out instance);
        }

        public bool TryGetModel(string name, out RoomModel model)
        {
            return this._roomModels.TryGetValue(name, out model);
        }

        public void Dispose()
        {
            lock (this._roomLoadingSync)
            {
                Dictionary<int, RoomInstance> ToUnload = new Dictionary<int, RoomInstance>(this._rooms);

                foreach (RoomInstance Instance in ToUnload.Values)
                {
                    Instance.Unload();
                }
            }
        }
    }
}
