using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class SitEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            Session.GetPlayer().GetAvatar().ForceSit = true;
        }
    }
}
