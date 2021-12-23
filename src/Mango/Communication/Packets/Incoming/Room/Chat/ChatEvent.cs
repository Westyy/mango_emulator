using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Room.Chat;

namespace Mango.Communication.Packets.Incoming.Room.Chat
{
    class ChatEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            string Message = StringCharFilter.Escape(packet.PopString());

            if (Message.Length > 100)
            {
                Message = Message.Substring(0, 100);
            }

            int Colour = packet.PopInt();

            if (!session.GetPlayer().GetPermissions().HasRight("club_vip") && Colour != 0
                || Colour == 23 && !session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                Colour = 0;
            }

            session.GetPlayer().GetAvatar().Chat(Message, Colour);
        }
    }
}
