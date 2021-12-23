using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Settings
{
    class FlatControllerAddedComposer : ServerPacket
    {
        public FlatControllerAddedComposer(int RoomId, int UserId, string Username)
            : base(ServerPacketHeadersNew.FlatControllerAddedComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(UserId);
            base.WriteString(Username);
        }
    }
}
