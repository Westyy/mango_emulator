using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;

namespace Mango.Rooms
{
    sealed class RoomPromotion
    {
        private const int MINUTES_RUNTIME = 120;

        private string _name;
        private string _description;
        private double _timestampExpires;
        private double _timestampStarted;

        public RoomPromotion(double TimestampExpires, double TimestampStarted)
        {
            this._name = "omg promote!";
            this._description = "omg yay";
            this._timestampExpires = (TimestampExpires += (MINUTES_RUNTIME * 60));
            this._timestampStarted = TimestampStarted;
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

        public double TimestampExpires
        {
            get { return this._timestampExpires; }
            set { this._timestampExpires = value; }
        }

        public double TimestampStarted
        {
            get { return this._timestampStarted; }
        }

        public bool HasExpired
        {
            get { return (this.TimestampExpires - UnixTimestamp.GetNow()) < 0; }
        }

        public int MinutesLeft
        {
            get { return Convert.ToInt32(Math.Ceiling((this.TimestampExpires - UnixTimestamp.GetNow()) / 60)); }
        }
    }
}
