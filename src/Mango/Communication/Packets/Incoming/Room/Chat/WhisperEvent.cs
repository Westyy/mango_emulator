using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Utilities;

namespace Mango.Communication.Packets.Incoming.Room.Chat
{
    class WhisperEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            string Message = StringCharFilter.Escape(packet.PopString());

            if (Message.Length < 1)
            {
                return;
            }

            if (Message.Length > 100)
            {
                Message = Message.Substring(0, 100);
            }

            string[] Bits = Message.Split(' ');

            if (Bits.Length < 2)
            {
                return;
            }

            string Username = Bits[0];
            Message = Message.Substring(Username.Length + 1);

            if (Username.Length < 1)
            {
                return;
            }

            int Colour = packet.PopInt();

            if (!session.GetPlayer().GetPermissions().HasRight("club_vip") && Colour != 0
                || Colour == 23 && !session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                Colour = 0;
            }

            session.GetPlayer().GetAvatar().Whisper(Message, Username, Colour);
        }
    }
}
