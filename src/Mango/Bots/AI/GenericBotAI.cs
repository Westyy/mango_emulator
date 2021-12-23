using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Bots.AI
{
    sealed class GenericBotAI : IBotAI
    {
        private readonly Bot _bot;

        public void OnUpdateTick(RoomInstance Instance)
        {

        }

        public void OnEnterRoom(RoomInstance Instance)
        {
            throw new NotImplementedException();
        }

        public void OnLeaveRoom(RoomInstance Instance)
        {
            throw new NotImplementedException();
        }

        public void OnUserTalks(RoomInstance Instance, RoomAvatar Avatar, string Message)
        {
            throw new NotImplementedException();
        }

        public void OnUserEnter(RoomInstance Instance, RoomAvatar Avatar)
        {
            throw new NotImplementedException();
        }

        public void OnUserLeave(RoomInstance Instance, RoomAvatar Avatar)
        {
            throw new NotImplementedException();
        }
    }
}
