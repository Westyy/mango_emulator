using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Action
{
    class DanceComposer : ServerPacket
    {
        public DanceComposer(RoomAvatar Avatar, int DanceId)
            : base(ServerPacketHeadersNew.DanceMessageComposer)
        {
            base.WriteInteger(Avatar.Data.Id);
            base.WriteInteger(DanceId);
        }
    }
}
