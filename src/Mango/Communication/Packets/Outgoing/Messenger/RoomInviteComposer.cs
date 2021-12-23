using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class RoomInviteComposer : ServerPacket
    {
        public RoomInviteComposer(int SenderId, string Text)
            : base(ServerPacketHeadersNew.RoomInviteComposer)
        {
            base.WriteInteger(SenderId);
            base.WriteString(Text);
        }
    }
}
