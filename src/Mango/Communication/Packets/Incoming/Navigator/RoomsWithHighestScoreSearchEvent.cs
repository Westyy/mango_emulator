using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Rooms;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class RoomsWithHighestScoreSearchEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            List<RoomData> rooms = Mango.GetServer().GetRoomManager().GetPopularRatedRooms();
            session.SendPacket(new GuestRoomSearchResultComposer(0, 2, string.Empty, rooms));
        }
    }
}
