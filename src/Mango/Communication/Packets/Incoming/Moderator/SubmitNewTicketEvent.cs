using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class SubmitNewTicketEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            // todo: this
            string Message = StringCharFilter.Escape(Packet.PopString().Trim());
            int Junk = Packet.PopWiredInt();
            int Type = Packet.PopWiredInt();
            int ReportedUser = Packet.PopWiredInt();
        }
    }
}
