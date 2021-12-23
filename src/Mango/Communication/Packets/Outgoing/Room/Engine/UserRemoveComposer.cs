using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int userId)
            : base(ServerPacketHeadersNew.UserRemoveMessageComposer)
        {
            base.WriteRawInteger(userId);
        }
    }
}
