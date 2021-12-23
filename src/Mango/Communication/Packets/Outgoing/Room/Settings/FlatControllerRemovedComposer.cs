using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Settings
{
    class FlatControllerRemovedComposer : ServerPacket
    {
        public FlatControllerRemovedComposer(RoomInstance Instance, int UserId)
            : base(ServerPacketHeadersNew.FlatControllerRemovedComposer)
        {
            base.WriteInteger(Instance.Id);
            base.WriteInteger(UserId);
        }
    }
}
