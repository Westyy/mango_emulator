using Mango.Communication.Packets.Outgoing.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            // todo: this
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int UserId = Packet.PopWiredInt();

            Session.SendPacket(new ModeratorUserRoomVisitsComposer());
        }
    }
}
