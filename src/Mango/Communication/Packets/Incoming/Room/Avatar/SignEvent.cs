using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class SignEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            int SignId = Packet.PopWiredInt();

            Session.GetPlayer().GetAvatar().ApplySign(SignId);
        }
    }
}
