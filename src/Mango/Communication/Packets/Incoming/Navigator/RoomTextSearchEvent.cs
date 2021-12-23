using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Communication.Packets.Outgoing.Moderation;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class RoomTextSearchEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            string Query = StringCharFilter.Escape(packet.PopString()).ToLower().Trim();

            bool ByOwnerUsername = false;
            string OwnerUsername = "";

            if (Query.Split(':').Length > 1)
            {
                ByOwnerUsername = true;
                OwnerUsername = Query.Split(':')[1];
            }

            if (Query.Length > 64)
            {
                Query = Query.Substring(0, 64);
            }

            if (Query.Length < 0)
            {
                return;
            }

            /*Dictionary<string, int> EventSearchQueries = Mango.GetServer().GetNavigatorManager().GetSearchEventQueries();
            int SearchEventCataId = 0;

            if (EventSearchQueries.ContainsKey(Query.ToLower()))
            {
                EventSearchQueries.TryGetValue(Query.ToLower(), out SearchEventCataId);
            }*/

            Dictionary<int, RoomData> Rooms = new Dictionary<int, RoomData>();

            List<RoomData> LoadedRooms = ByOwnerUsername ? null : Mango.GetServer().GetRoomManager().SearchRooms(Query);

            if (LoadedRooms != null)
            {
                foreach (RoomData data in LoadedRooms)
                {
                    Rooms.Add(data.Id, data);
                }
            }

            if (Rooms.Count < 50) // need more, lets ask the db :-D
            {
                List<RoomData> MoreRooms = ByOwnerUsername ? RoomLoader.SearchForRoomsByOwnerName(OwnerUsername, (50 - Rooms.Count)) : RoomLoader.SearchForRooms(Query, (50 - Rooms.Count));

                foreach (RoomData room in MoreRooms)
                {
                    if (!Rooms.ContainsKey(room.Id))
                    {
                        Rooms.Add(room.Id, room);
                    }
                }
            }

            session.SendPacket(new GuestRoomSearchResultComposer(1, 9, Query, Rooms.Values));
        }
    }
}
