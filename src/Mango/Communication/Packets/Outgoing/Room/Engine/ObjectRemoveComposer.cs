using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ObjectRemoveComposer : ServerPacket
    {
        public ObjectRemoveComposer(Item item, int UserId)
            : base(ServerPacketHeadersNew.ObjectRemoveMessageComposer)
        {
            base.WriteRawInteger(item.Id);
            base.WriteInteger(0);
            base.WriteInteger(UserId);
        }
    }
}
