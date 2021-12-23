using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Session
{
    class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(RoomData Data, RoomModel Model)
            : base(ServerPacketHeadersNew.RoomReadyMessageComposer)
        {
            base.WriteString(Model.Id);
            base.WriteInteger(Data.Id);
        }
    }
}
