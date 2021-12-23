using Mango.Database.Exceptions;
using Mango.Rooms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Favourites
{
    sealed class FavouritesComponent
    {
        private ConcurrentDictionary<int, FavouriteRoom> _favouriteRooms;

        public FavouritesComponent()
        {
            this._favouriteRooms = new ConcurrentDictionary<int, FavouriteRoom>();
        }

        public bool Init(Player Player)
        {
            if (_favouriteRooms.Count > 0)
            {
                throw new InvalidOperationException("Cannot re-initialize the favourites componenet.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `user_favourites` WHERE `user_id` = @uid;");
                DbCon.AddParameter("uid", Player.Id);
                DbCon.Open();

                try
                {
                    using (MySqlDataReader Reader = DbCon.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            this._favouriteRooms.TryAdd(Reader.GetInt32("room_id"), new FavouriteRoom(Reader.GetInt32("id"), Reader.GetInt32("room_id"), Reader.GetInt32("user_id")));
                        }
                    }
                }
                catch (DatabaseException)
                {
                    return false;
                }
                catch (MySqlException)
                {
                    return false;
                }
            }

            return true;
        }

        public ICollection<int> FavouriteRoomsId
        {
            get { return this._favouriteRooms.Keys; }
        }

        public bool TryAdd(FavouriteRoom Room)
        {
            return this._favouriteRooms.TryAdd(Room.RoomId, Room);
        }

        public bool TryRemove(int RoomId, out FavouriteRoom Removed)
        {
            return this._favouriteRooms.TryRemove(RoomId, out Removed);
        }
    }
}
