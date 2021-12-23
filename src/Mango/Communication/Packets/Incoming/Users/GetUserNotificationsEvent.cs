using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Users
{
    class GetUserNotificationsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
        }
    }
}
