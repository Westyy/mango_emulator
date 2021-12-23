using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Bots.AI
{
    interface IBotAI
    {
        void OnUpdateTick(RoomInstance Instance);
        void OnEnterRoom(RoomInstance Instance);
        void OnLeaveRoom(RoomInstance Instance);
        void OnUserTalks(RoomInstance Instance, RoomAvatar Avatar, string Message);
        void OnUserEnter(RoomInstance Instance, RoomAvatar Avatar);
        void OnUserLeave(RoomInstance Instance, RoomAvatar Avatar);
    }
}
