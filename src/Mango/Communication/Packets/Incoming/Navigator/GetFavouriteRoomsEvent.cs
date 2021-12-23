using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetFavouriteRoomsEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            List<RoomData> FavouritesData = RoomLoader.GetRoomsForIds(Session.GetPlayer().Favourites().FavouriteRoomsId);
            Session.SendPacket(new GuestRoomSearchResultComposer(0, 6, string.Empty, FavouritesData));
        }
    }
}
