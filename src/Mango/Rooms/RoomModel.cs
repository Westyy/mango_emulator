using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Subscriptions;
using Mango.Players;
using Mango.Database.Exceptions;

namespace Mango.Rooms
{
    sealed class RoomModel
    {
        private string _id;
        private RoomModelType _type;
        private Heightmap _heightmap;
        private int _doorX;
        private int _doorY;
        private double _doorZ;
        private int _doorRotation;
        private int _maxUsers;
        private SubscriptionLevel _levelRequired;

        public RoomModel(string Id, string Type, Heightmap Heightmap, int DoorX, int DoorY, double DoorZ, int DoorRotation, int MaxUsers, string Level)
        {
            this._id = Id;

            if (Type != "flat" && Type != "public")
                throw new DatabaseException(string.Format("Expected data to be 'flat' or 'public' but was '{0}'.", Type));

            switch (Type)
            {
                case "flat":
                    this._type = RoomModelType.FLAT;
                    break;

                case "public":
                    this._type = RoomModelType.PUBLIC;
                    break;
            }

            this._heightmap = Heightmap;
            this._doorX = DoorX;
            this._doorY = DoorY;
            this._doorZ = DoorZ;
            this._doorRotation = DoorRotation;
            this._maxUsers = MaxUsers;

            if (Level != "0" && Level != "1" && Level != "2")
                throw new DatabaseException(string.Format("Expected data to be '0' or '1' or '2' but was '{0}'.", Level));

            switch (Level)
            {
                case "0":
                    this._levelRequired = SubscriptionLevel.NONE;
                    break;

                case "1":
                    this._levelRequired = SubscriptionLevel.BASIC;
                    break;

                case "2":
                    this._levelRequired = SubscriptionLevel.VIP;
                    break;
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
        }

        public RoomModelType Type
        {
            get
            {
                return this._type;
            }
        }

        public Heightmap Heightmap
        {
            get
            {
                return this._heightmap;
            }
        }

        public int DoorX
        {
            get
            {
                return this._doorX;
            }
        }

        public int DoorY
        {
            get
            {
                return this._doorY;
            }
        }

        public double DoorZ
        {
            get
            {
                return this._doorZ;
            }
        }

        public int DoorRotation
        {
            get
            {
                return this._doorRotation;
            }
        }

        public int MaxUsers
        {
            get
            {
                return this._maxUsers;
            }
        }

        public SubscriptionLevel LevelRequired
        {
            get
            {
                return this._levelRequired;
            }
        }

        public bool IsUsableByPlayer(Player Player)
        {
            if (this._type == RoomModelType.PUBLIC)
            {
                return false;
            }

            switch (_levelRequired)
            {
                default:
                case SubscriptionLevel.NONE:
                    return true;

                case SubscriptionLevel.BASIC:
                    return (Player.GetPermissions().HasRight("club_regular") || Player.GetPermissions().HasRight("club_vip"));

                case SubscriptionLevel.VIP:
                    return (Player.GetPermissions().HasRight("club_vip"));
            }
        }
    }
}
