using System;
using System.Text;
using System.Text.RegularExpressions;
using Mango.Rooms.Mapping;

namespace Mango.Rooms
{
    sealed class Heightmap
    {
        private RoomTileState[,] _tileStates;
        private int[,] _floorHeight;
        private int _sizeX;
        private int _sizeY;

        private string _heightMap;

        public Heightmap(string Heightmap)
        {
            string[] map = Regex.Split(Heightmap, "\r\n");

            this._sizeX = map[0].Length;
            this._sizeY = map.Length;

            this._tileStates = new RoomTileState[this._sizeX, this._sizeY];
            this._floorHeight = new int[this._sizeX, this._sizeY];

            for (int y = 0; y < this._sizeY; y++)
            {
                for (int x = 0; x < this._sizeX; x++)
                {
                    string value = map[y][x].ToString().ToLower();

                    if (value == "x")
                    {
                        this._tileStates[x, y] = RoomTileState.Blocked;
                        this._floorHeight[x, y] = 0;

                        this._heightMap += "x";
                    }
                    else
                    {
                        int FhDig = int.Parse(value);

                        this._tileStates[x, y] = RoomTileState.Open;
                        this._floorHeight[x, y] = FhDig;

                        this._heightMap += FhDig;
                    }
                }

                this._heightMap += Convert.ToChar(13);
            }
        }

        public override string ToString()
        {
            return this._heightMap;
        }

        public RoomTileState[,] TileStates
        {
            get
            {
                return this._tileStates;
            }
        }

        public int[,] FloorHeight
        {
            get
            {
                return this._floorHeight;
            }
        }

        public int SizeX
        {
            get
            {
                return this._sizeX;
            }
        }

        public int SizeY
        {
            get
            {
                return this._sizeY;
            }
        }
    }
}
