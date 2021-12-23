using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class RoomEntryInfoComposer : ServerPacket
    {
        public RoomEntryInfoComposer(RoomData data, bool HasOwnerRights)
            : base(ServerPacketHeadersNew.RoomEntryInfoMessageComposer)
        {
            base.WriteBoolean(data.Type == RoomType.FLAT ? true : false);
            base.WriteInteger(data.Id);
            base.WriteBoolean(HasOwnerRights);
        }
    }
}
