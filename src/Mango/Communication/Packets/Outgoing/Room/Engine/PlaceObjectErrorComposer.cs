using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class PlaceObjectErrorComposer : ServerPacket
    {
        public PlaceObjectErrorComposer(ItemPlacementError error)
            : base(ServerPacketHeadersNew.PlaceObjectErrorMessageComposer)
        {
            base.WriteInteger(ItemPlacementErrorUtility.GetPlacementErrorNum(error));
        }
    }
}
