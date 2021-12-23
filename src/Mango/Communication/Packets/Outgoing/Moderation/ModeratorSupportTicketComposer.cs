using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketComposer : ServerPacket
    {
        public ModeratorSupportTicketComposer()
            : base(ServerPacketHeadersNew.ModeratorSupportTicketComposer)
        {
            base.WriteInteger(1); // Id
            base.WriteInteger(1); // Tab ID
            base.WriteInteger(3); // Type
            base.WriteInteger(114); // Category
            base.WriteInteger(0); // time in milliseconds?
            base.WriteInteger(1); // Priority
            base.WriteInteger(1); // Sender ID
            base.WriteString("Matty"); // Sender Name
            base.WriteInteger(2); // Reported ID
            base.WriteString("Tom"); // Reported Name
            base.WriteInteger(0); // Moderator ID
            base.WriteString(""); // Mod Name
            base.WriteString("He tells me to go naked on tinychat with him!!"); // Issue
            base.WriteInteger(5); // Room Id
            base.WriteString("Sex Dungeon"); // Room Name
            base.WriteInteger(0); // Is Public room
            base.WriteString("-1");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
