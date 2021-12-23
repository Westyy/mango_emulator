using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(RoomInstance Instance)
            : base(ServerPacketHeadersNew.ModeratorRoomChatlogComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(Instance.Id);
            base.WriteString(Instance.Name);
            base.WriteInteger(0); // Chatlog count
            {
                base.WriteInteger(0); // time?
                base.WriteInteger(Instance.OwnerId); // User Id
                base.WriteString(Instance.OwnerName); // Username
                base.WriteString("Hello chatlogs!"); // Message
            }
        }
    }
}
