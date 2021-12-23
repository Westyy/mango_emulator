using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ObjectDataUpdateComposer : ServerPacket
    {
        public ObjectDataUpdateComposer(Item item)
            : base(ServerPacketHeadersNew.ObjectDataUpdateMessageComposer)
        {
            base.WriteString(item.Id.ToString());
            base.WriteInteger(0); // Unknown
            base.WriteString(item.Flags);
        }
    }
}
