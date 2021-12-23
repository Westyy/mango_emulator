using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Inventory.Purse;
using Mango.Communication.Packets.Outgoing.Notifications;

namespace Mango.Communication.Packets.Incoming.Inventory.Purse
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new CreditBalanceComposer(session.GetPlayer().Credits));
            session.SendPacket(new ActivityPointsComposer(session.GetPlayer().Pixels));
        }
    }
}
