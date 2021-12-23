using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ItemUpdateComposer : ServerPacket
    {
        public ItemUpdateComposer(Item item, int UserId)
            : base(ServerPacketHeadersNew.ItemUpdateMessageComposer)
        {
            WriteWallItem(item, UserId);
        }

        private void WriteWallItem(Item item, int UserId)
        {
            base.WriteString(item.Id.ToString());
            base.WriteInteger(item.Data.SpriteId);
            base.WriteString(item.RoomWallPos);
            base.WriteString(item.DisplayFlags);
            base.WriteInteger(item.Usable ? 1 : 0);
            base.WriteInteger(UserId);
        }
    }
}
