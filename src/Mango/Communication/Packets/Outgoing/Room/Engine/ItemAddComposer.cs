using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ItemAddComposer : ServerPacket
    {
        public ItemAddComposer(Item item, string Username)
            : base(ServerPacketHeadersNew.ItemAddMessageComposer)
        {
            WriteWallItem(item, Username);
        }

        private void WriteWallItem(Item item, string Username)
        {
            base.WriteString(item.Id.ToString());
            base.WriteInteger(item.Data.SpriteId);
            base.WriteString(item.RoomWallPos);
            base.WriteString(item.DisplayFlags);
            base.WriteInteger(item.Usable ? 1 : 0);
            base.WriteInteger(item.UserId);
            base.WriteString(Username);
        }
    }
}
