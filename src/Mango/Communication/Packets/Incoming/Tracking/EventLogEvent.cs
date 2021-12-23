using log4net;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Tracking
{
    class EventLogEvent : IPacketEvent
    {
        private static ILog log = LogManager.GetLogger("Mango.Communication.Packets.Tracking.EventLogEvent");

        public void parse(Session session, ClientPacket packet)
        {
            //log.Debug(packet.PopString());
        }
    }
}
