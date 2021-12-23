using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Handshake;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class InfoRetrieveEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new UserObjectComposer(session.GetPlayer()));
            session.SendPacket(new UserPerksComposer());
        }
    }
}
