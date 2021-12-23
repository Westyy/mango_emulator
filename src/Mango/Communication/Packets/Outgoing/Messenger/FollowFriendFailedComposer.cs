using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class FollowFriendFailedComposer : ServerPacket
    {
        public FollowFriendFailedComposer()
            : base(ServerPacketHeadersNew.FollowFriendFailedComposer)
        {
        }
    }
}
