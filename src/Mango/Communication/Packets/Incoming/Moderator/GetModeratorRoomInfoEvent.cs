using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int RoomId = packet.PopWiredInt();

            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (session.GetPlayer().GetAvatar().GetCurrentRoom().Id != RoomId)
            {
                return;
            }
            //todo: Make this work across other rooms!! (Room visits tool etc)
            RoomInstance CurrentRoom = session.GetPlayer().GetAvatar().GetCurrentRoom();

            session.SendPacket(new ModeratorRoomInfoComposer(CurrentRoom));
        }
    }
}
