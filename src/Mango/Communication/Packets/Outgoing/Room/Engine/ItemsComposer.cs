using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ItemsComposer : ServerPacket
    {
        public ItemsComposer(ICollection<Item> items, RoomInstance Instance)
            : base(ServerPacketHeadersNew.ItemsMessageComposer)
        {
            //todo: groups for room furni things

            base.WriteInteger(1); // Count of room owners
            base.WriteInteger(Instance.OwnerId);
            base.WriteString(Instance.OwnerName);

            base.WriteInteger(items.Count);

            foreach (Item item in items)
            {
                WriteWallItem(item, Instance.OwnerId);
            }
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
