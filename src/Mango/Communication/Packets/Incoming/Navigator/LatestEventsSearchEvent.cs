using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class LatestEventsSearchEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            /*int category = -1;
            int.TryParse(packet.PopString(), out category);

            List<RoomData> roomsWithEvents = Mango.GetServer().GetRoomManager().GetOnGoingRoomEvents(category);
            session.SendPacket(new GuestRoomSearchResultComposer(category, 12, string.Empty, roomsWithEvents, true));*/
        }
    }
}
