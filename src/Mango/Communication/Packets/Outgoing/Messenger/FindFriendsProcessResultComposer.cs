using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class FindFriendsProcessResultComposer : ServerPacket
    {
        public FindFriendsProcessResultComposer(bool Success)
            : base(ServerPacketHeadersNew.FindFriendsProcessResultComposer)
        {
            base.WriteBoolean(Success);
        }
    }
}
