using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    sealed class ItemStaticPublics
    {
        public string _name;
        public int _x;
        public int _y;
        public double _z;
        public int _rotation;
        public bool _isSeat;
        private bool _isBed;

        public ItemStaticPublics(string Name, int X, int Y, double Z, int Rotation, bool IsSeat, bool IsBed)
        {
            this._name = Name;
            this._x = X;
            this._y = Y;
            this._z = Z;
            this._rotation = Rotation;
            this._isSeat = IsSeat;
            this._isBed = IsBed;
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public int X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public double Z
        {
            get { return this._z; }
            set { this._z = value; }
        }

        public int Rotation
        {
            get { return this._rotation; }
            set { this._rotation = value; }
        }

        public bool IsSeat
        {
            get { return this._isSeat; }
            set { this._isSeat = value; }
        }

        public bool IsBed
        {
            get { return this._isBed; }
            set { this._isBed = value; }
        }
    }
}
