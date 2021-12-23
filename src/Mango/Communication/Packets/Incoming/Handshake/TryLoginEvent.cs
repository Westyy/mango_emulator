using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class TryLoginEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
        }
    }
}
