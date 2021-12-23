using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Mapping
{
    static class AvatarRotation
    {
        public static int Calculate(Vector2D PositionOne, Vector2D PositionTwo)
        {
            if (PositionOne.X > PositionTwo.X && PositionOne.Y > PositionTwo.Y)
            {
                return 7;
            }
            else if (PositionOne.X < PositionTwo.X && PositionOne.Y < PositionTwo.Y)
            {
                return 3;
            }
            else if (PositionOne.X > PositionTwo.X && PositionOne.Y < PositionTwo.Y)
            {
                return 5;
            }
            else if (PositionOne.X < PositionTwo.X && PositionOne.Y > PositionTwo.Y)
            {
                return 1;
            }
            else if (PositionOne.X > PositionTwo.X)
            {
                return 6;
            }
            else if (PositionOne.X < PositionTwo.X)
            {
                return 2;
            }
            else if (PositionOne.Y < PositionTwo.Y)
            {
                return 4;
            }
            else if (PositionOne.Y > PositionTwo.Y)
            {
                return 0;
            }

            return 0;
        }
    }
}
