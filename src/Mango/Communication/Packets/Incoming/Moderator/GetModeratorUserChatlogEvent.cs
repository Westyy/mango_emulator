using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            // todo: this
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            PlayerData Data = PlayerLoader.GetDataById(Packet.PopWiredInt());

            if (Data == null)
            {
                Session.SendPacket(new ModMessageComposer("Unable to load info for user."));
                return;
            }

            Session.SendPacket(new ModeratorUserChatlogComposer(Data));

        }
    }
}
