using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorIssueChatlogComposer : ServerPacket
    {
        public ModeratorIssueChatlogComposer()
            : base(ServerPacketHeadersNew.ModeratorIssueChatlogComposer)
        {
            base.WriteInteger(1); // Ticket Id
            base.WriteInteger(1); // Sender Id
            base.WriteInteger(1); // Reported Id
            base.WriteInteger(1); // Room Id
            base.WriteBoolean(false); // Public Room
            base.WriteInteger(1); // Room Id
            base.WriteString("HI"); // Room name

            base.WriteInteger(1); // Count
            {
                base.WriteInteger(1); // Time?
                base.WriteInteger(1); // User Id
                base.WriteString("Matty"); // Username
                base.WriteString("I love tom.. |"); // Message
            }
        }
    }
}
