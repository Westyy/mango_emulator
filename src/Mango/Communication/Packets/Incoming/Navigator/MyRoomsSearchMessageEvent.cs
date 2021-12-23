using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class MyRoomsSearchMessageEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            ICollection<RoomData> myRooms = RoomLoader.GetRoomsDataByOwnerIdSortByName(session.GetPlayer().Id);
            session.SendPacket(new GuestRoomSearchResultComposer(0, 5, string.Empty, myRooms));
        }
    }
}
