using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Navigator;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetOfficialRoomsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            ICollection<NavigatorOfficial> Featured = Mango.GetServer().GetNavigatorManager().GetFeaturedFlats();
            session.SendPacket(new OfficialRoomsComposer(Featured));
        }
    }
}
