using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class GetClientVersionEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();
        }
    }
}
