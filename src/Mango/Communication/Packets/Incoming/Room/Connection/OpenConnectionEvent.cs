using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Room.Connection
{
    class OpenConnectionEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            bool UnknownData1 = packet.PopWiredBoolean();
            int RoomId = packet.PopWiredInt();
            bool UnknownData2 = packet.PopWiredBoolean();

            session.GetPlayer().GetAvatar().PrepareRoom(RoomId, string.Empty);
        }
    }
}
