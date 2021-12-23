using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using log4net;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class UniqueIDEvent : IPacketEvent
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Communication.Packets.Incoming.Handshake.UniqueIDEvent");

        public void parse(Session Session, ClientPacket Packet)
        {
            string Junk = Packet.PopString();
            string MachineId = Packet.PopString();

            log.Debug("Session " + Session.Id + " - " + MachineId);
        }
    }
}
