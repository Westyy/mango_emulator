using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetUserFlatCatsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new UserFlatCatsComposer(Mango.GetServer().GetNavigatorManager().GetFlatCategories()));
        }
    }
}
