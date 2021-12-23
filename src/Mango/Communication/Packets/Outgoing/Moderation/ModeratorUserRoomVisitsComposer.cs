using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer()
            : base(ServerPacketHeadersNew.ModeratorUserRoomVisitsComposer)
        {
            base.WriteInteger(1); // UserId
            base.WriteString("Matty"); // Username

            base.WriteInteger(1); // Count
            {
                base.WriteBoolean(false); // Public room
                base.WriteInteger(1); // Room Id
                base.WriteString("Sex Dungeon"); // Room Name
                base.WriteInteger(1); // Hour
                base.WriteInteger(2); // Minute
            }
        }
    }
}
