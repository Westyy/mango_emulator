using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Room.Chat;

namespace Mango.Communication.Packets.Incoming.Room.Chat
{
    class StartTypingEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new UserTypingComposer(session.GetPlayer(), true));
        }
    }
}
