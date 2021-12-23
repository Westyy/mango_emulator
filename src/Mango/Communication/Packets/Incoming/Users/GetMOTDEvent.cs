
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Notifications;
using Mango.Communication.Packets.Outgoing.Moderation;

namespace Mango.Communication.Packets.Incoming.Users
{
    class GetMOTDEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            //session.SendPacket(new MOTDNotificationComposer("Changes:\r\n- Fixed login errors\r\n- Fixed crashes on user logout"));
            session.SendPacket(new MOTDNotificationComposer("Mango Server, please report any bugs you find.\r\n\r\nVersion: " + Mango.Version));
            //session.SendPacket(new ModMessageComposer("Please click more information below to report bugs to our Uservoice.\r\nFeel free to also post your suggestions, we love feedback!", "https://mangoserver.uservoice.com/"));
        }
    }
}
