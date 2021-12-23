using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Handshake;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class PongEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            //session.SendPacket(new PingComposer()); // don't reply with ping again
        }
    }
}
