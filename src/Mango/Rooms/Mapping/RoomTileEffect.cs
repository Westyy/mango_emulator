using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Mapping
{
    sealed class RoomTileEffect
    {
        private RoomTileEffectType _type;
        private int _effectId;
        private int _x;
        private int _y;
        private int _rot;
        private double _interactHeight;
        private int _questData;

        public RoomTileEffect()
        {
            this._type = RoomTileEffectType.None;
            this._effectId = -1;
            this._x = 0;
            this._y = 0;
            this._rot = 0;
            this._interactHeight = -1;
            this._questData = 0;
        }

        public RoomTileEffect(RoomTileEffectType type, int x, int y, int rot, double interactHeight, int effectId = 0, int questData = 0)
        {
            this._type = type;
            this._x = x;
            this._y = y;
            this._rot = rot;
            this._interactHeight = interactHeight;
            this._effectId = effectId;
            this._questData = questData;
        }

        public RoomTileEffectType Type
        {
            get
            {
                return this._type;
            }
        }

        public int EffectId
        {
            get
            {
                return this._effectId;
            }
        }

        public int X
        {
            get
            {
                return this._x;
            }
        }

        public int Y
        {
            get
            {
                return this._y;
            }
        }

        public int Rot
        {
            get
            {
                return this._rot;
            }
        }

        public double InteractHeight
        {
            get
            {
                return this._interactHeight;
            }
        }

        public int QuestData
        {
            get
            {
                return this._questData;
            }
        }
    }
}
