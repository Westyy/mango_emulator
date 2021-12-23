using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Room.Connection
{
    class OpenFlatConnectionEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int RoomId = packet.PopWiredInt();
            string Password = packet.PopString();

            session.GetPlayer().GetAvatar().PrepareRoom(RoomId, Password);
        }
    }
}
