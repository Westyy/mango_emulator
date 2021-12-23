using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetPopularRoomTagsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new PopularRoomTagsResultComposer(Mango.GetServer().GetRoomManager().GetPopularRoomTags()));
        }
    }
}
