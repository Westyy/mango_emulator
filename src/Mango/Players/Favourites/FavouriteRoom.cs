using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Favourites
{
    sealed class FavouriteRoom
    {
        private int _id;
        private int _roomId;
        private int _userId;

        public FavouriteRoom(int Id, int RoomId, int UserId)
        {
            this.Id = Id;
            this.RoomId = RoomId;
            this.UserId = UserId;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int RoomId
        {
            get { return this._roomId; }
            set { this._roomId = value; }
        }

        public int UserId
        {
            get { return this._userId; }
            set { this._userId = value; }
        }
    }
}
