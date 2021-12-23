using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Packets.Outgoing.Room.Session;

namespace Mango.Communication.Packets.Incoming.Room.Connection
{
    class GoToFlatEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (!Session.GetPlayer().GetAvatar().EnterRoom())
            {
                Session.SendPacket(new CloseConnectionComposer());
            }
        }
    }
}
