using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Mapping
{
    static class DistanceCalculator
    {
        public static int Calculate(Vector2D Position1, Vector2D Position2)
        {
            return Math.Abs(Position1.X - Position2.X) + Math.Abs(Position1.Y - Position2.Y);
        }

        public static bool TilesTouching(Vector2D Position1, Vector2D Position2)
        {
            /*return (Calculate(Position1, Position2) <= 1);*/
            if (!(Math.Abs(Position1.X - Position2.X) > 1 || Math.Abs(Position1.Y - Position2.Y) > 1)) return true;
            if (Position1.X == Position2.X && Position1.Y == Position2.Y) return true;
            return false;
        }
    }
}
