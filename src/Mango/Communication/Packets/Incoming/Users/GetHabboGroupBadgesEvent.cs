using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Users;

namespace Mango.Communication.Packets.Incoming.Users
{
    class GetHabboGroupBadgesEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (instance == null)
            {
                return;
            }

            //session.SendPacket(new HabboGroupBadgesComposer()); // to-do: send blank data d/c
        }
    }
}
