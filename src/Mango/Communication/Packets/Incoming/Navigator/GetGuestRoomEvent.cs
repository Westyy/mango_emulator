using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetGuestRoomEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomData data = null;

            if (!RoomLoader.TryGetData(packet.PopWiredInt(), out data))
            {
                return;
            }

            int dunno = packet.PopWiredInt();
            bool bool1 = packet.PopWiredInt() == 1 ? true : false; // true when entering a room, otherwise always false
            bool bool2 = packet.PopWiredInt() == 1 ? true : false; // true when requesting info before entering (stalking etc), otherwise always false??

            session.SendPacket(new GetGuestRoomResultComposer(data, bool2, bool1));
        }
    }
}
