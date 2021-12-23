using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class PopularRoomsSearchEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int categoryId = -1;
            int.TryParse(packet.PopString(), out categoryId);

            List<RoomData> rooms = Mango.GetServer().GetRoomManager().GetPopularRooms(categoryId);
            session.SendPacket(new GuestRoomSearchResultComposer(categoryId, 1, string.Empty, rooms));
        }
    }
}
