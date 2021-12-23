using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Handshake;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class GetSessionParametersEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            string Swf = packet.PopString();
            string Vars = packet.PopString();
        }
    }
}
