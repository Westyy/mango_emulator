using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class PromotedRoomsEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            int Something = Packet.PopWiredInt();
            int CategoryId = Packet.PopWiredInt();

            List<RoomData> Rooms = Mango.GetServer().GetRoomManager().GetOnGoingRoomPromotions(CategoryId);
            Session.SendPacket(new GuestRoomSearchResultComposer(0, CategoryId, string.Empty, Rooms, true));
        }
    }
}
