using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Mapping
{
    [Obsolete("No longer required")]
    sealed class PositionUpdate
    {
        private readonly Vector3D _position;
        private readonly int _rotation;

        public PositionUpdate(Vector3D Position, int Rotation)
        {
            this._position = Position;
            this._rotation = Rotation;
        }

        public Vector3D Position
        {
            get { return this._position; }
        }

        public int Rotation
        {
            get { return this._rotation; }
        }
    }
}
