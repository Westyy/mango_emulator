using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserChatlogComposer : ServerPacket
    {
        public ModeratorUserChatlogComposer(PlayerData Data)
            : base(ServerPacketHeadersNew.ModeratorUserChatlogComposer)
        {
            base.WriteInteger(Data.Id);
            base.WriteString(Data.Username);

            base.WriteInteger(1); // Room Visits Count
            {
                base.WriteBoolean(false); // Public room
                base.WriteInteger(5); // Room Id
                base.WriteString("HALLO"); // Room Name

                base.WriteInteger(1); // Chatlogs Count
                {
                    base.WriteInteger(1); // Some time thing?
                    base.WriteInteger(Data.Id); // UserId of message
                    base.WriteString(Data.Username); // Username of message
                    base.WriteString("LOL"); // Message
                }
            }
        }
    }
}
