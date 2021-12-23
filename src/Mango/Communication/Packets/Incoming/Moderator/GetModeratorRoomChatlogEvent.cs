using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            // todo: this
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int Junk = Packet.PopInt();
            int RoomId = Packet.PopInt();

            // todo: Make this work across other rooms via room info tool
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (Session.GetPlayer().GetAvatar().GetCurrentRoom().Id != RoomId)
            {
                return;
            }
            RoomInstance CurrentRoom = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            Session.SendPacket(new ModeratorRoomChatlogComposer(CurrentRoom));
        }
    }
}
