using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Advertisement;

namespace Mango.Communication.Packets.Incoming.Advertisement
{
    class GetRoomAdEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            //session.SendPacket(new RoomAdComposer()); // old ad code
            session.SendPacket(new InterstitialComposer());
        }
    }
}
