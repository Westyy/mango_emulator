using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Settings
{
    class DeleteRoomEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().InRoom)
            {
                return;
            }

            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            Instance.DeleteRoom(Session);
        }
    }
}
