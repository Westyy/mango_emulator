using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Rooms.Mapping;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class MoveAvatarEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            int reqX = Packet.PopWiredInt();
            int reqY = Packet.PopWiredInt();

            if (reqX < 0 || reqY < 0)
            {
                return;
            }

            Session.GetPlayer().GetAvatar().MoveTo(new Vector2D(reqX, reqY));
        }
    }
}
