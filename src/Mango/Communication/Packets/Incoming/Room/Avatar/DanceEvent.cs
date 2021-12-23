using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms.Avatar;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class DanceEvent : IPacketEvent
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

            int DanceId = packet.PopWiredInt();

            if (DanceId < 0 || DanceId > 4)
            {
                return;
            }

            if (!session.GetPlayer().GetPermissions().HasRight("club_regular") && !session.GetPlayer().GetPermissions().HasRight("club_vip") && DanceId != 0)
            {
                DanceId = 1;
            }

            session.GetPlayer().GetAvatar().Dance(DanceId);
        }
    }
}
