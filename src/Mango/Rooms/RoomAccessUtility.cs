using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms
{
    static class RoomAccessUtility
    {
        public static int GetRoomAccessPacketNum(RoomAccess access)
        {
            switch (access)
            {
                default:
                case RoomAccess.Open:
                    return 0;

                case RoomAccess.Locked:
                    return 1;

                case RoomAccess.Password_Protected:
                    return 2;
            }
        }

        public static RoomAccess ToRoomAccess(int id)
        {
            switch (id)
            {
                default:
                case 0:
                    return RoomAccess.Open;

                case 1:
                    return RoomAccess.Locked;

                case 2:
                    return RoomAccess.Password_Protected;
            }
        }
    }
}
