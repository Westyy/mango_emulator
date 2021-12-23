using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Action
{
    class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(RoomAvatar Avatar, int CarryItemId)
            : base(ServerPacketHeadersNew.CarryObjectMessageComposer)
        {
            base.WriteInteger(Avatar.Data.Id);
            base.WriteInteger(CarryItemId);
        }
    }
}
