using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(Item item)
            : base(ServerPacketHeadersNew.FurniListRemoveComposer)
        {
            base.WriteInteger(item.Id);
        }
    }
}
