using Mango.Communication.Packets.Outgoing.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class OpenHelpToolEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            Session.SendPacket(new OpenHelpToolComposer());
        }
    }
}
