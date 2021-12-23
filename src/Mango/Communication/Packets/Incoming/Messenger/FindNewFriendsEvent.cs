using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Messenger;
using Mango.Communication.Packets.Outgoing.Room.Session;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class FindNewFriendsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance Instance = Mango.GetServer().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
            {
                session.SendPacket(new FindFriendsProcessResultComposer(true));
                session.SendPacket(new RoomForwardComposer(Instance));
            }
            else
            {
                session.SendPacket(new FindFriendsProcessResultComposer(false));
            }
        }
    }
}
