using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Messenger;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class FriendListUpdateEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.GetPlayer().GetMessenger().ProcessUpdates();
        }
    }
}
