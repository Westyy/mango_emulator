using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ItemDataUpdateComposer : ServerPacket
    {
        public ItemDataUpdateComposer(Item item)
            : base(ServerPacketHeadersNew.ItemDataUpdateMessageComposer)
        {
            base.WriteString(item.Id.ToString());
            base.WriteString(item.Flags);
        }
    }
}
