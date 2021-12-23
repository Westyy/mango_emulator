using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ObjectAddComposer : ServerPacket
    {
        public ObjectAddComposer(Item Item, RoomInstance Instance)
            : base(ServerPacketHeadersNew.ObjectAddMessageComposer)
        {
            WriteFloorItem(Item, Instance);
        }

        private void WriteFloorItem(Item Item, RoomInstance Instance)
        {
            int s = 0;

            if (Item.Data.Behaviour == ItemBehaviour.MUSIC_DISK)
            {
                int.TryParse(Item.DisplayFlags, out s);
            }

            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.Data.SpriteId);
            base.WriteInteger(Item.Position.X);
            base.WriteInteger(Item.Position.Y);
            base.WriteInteger(Item.RoomRot);
            base.WriteDouble(Item.Position.Z);
            base.WriteInteger(1);
            base.WriteInteger(s); // secondary
            base.WriteString(Item.DisplayFlags);
            base.WriteInteger(Item.Expires ? (Item.Expired ? (int)((double)(Math.Ceiling(Item.ExpireTimeLeft / 60))) + 1 : 0) : 0); // todo: check
            base.WriteInteger(Item.Usable ? 1 : 0);
            base.WriteInteger(Instance.OwnerId);
            base.WriteString(Instance.OwnerName);
        }
    }
}
