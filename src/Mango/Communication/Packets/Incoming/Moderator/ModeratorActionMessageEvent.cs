using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class ModeratorActionMessageEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomInstance CurrentRoom = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            int Unknown1 = Packet.PopWiredInt();
            int AlertMode = Packet.PopWiredInt();
            string AlertMessage = Packet.PopString();
            bool IsCaution = AlertMode != 3;

            AlertMessage = IsCaution ? "Caution from Moderator:\n\n" + AlertMessage : "Message from Moderator:\n\n" + AlertMessage;

            CurrentRoom.GetAvatars().BroadcastPacket(new ModMessageComposer(AlertMessage));
        }
    }
}
