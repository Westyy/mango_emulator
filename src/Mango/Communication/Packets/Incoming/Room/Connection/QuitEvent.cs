using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Room.Connection
{
    class QuitEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            session.GetPlayer().GetAvatar().LeaveRoom();
        }
    }
}
