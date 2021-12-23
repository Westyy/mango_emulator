using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ItemRemoveComposer : ServerPacket
    {
        public ItemRemoveComposer(Item item, int UserId)
            : base(ServerPacketHeadersNew.ItemRemoveMessageComposer)
        {
            base.WriteRawInteger(item.Id);
            base.WriteInteger(UserId);
        }
    }
}
