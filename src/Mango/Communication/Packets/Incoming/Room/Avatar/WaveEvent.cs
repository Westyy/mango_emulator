using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Room.Action;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class WaveEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (session.GetPlayer().GetAvatar().EffectId > 0)
            {
                return;
            }

            int Action = packet.PopWiredInt();
            session.GetPlayer().GetAvatar().Wave(Action);
        }
    }
}
