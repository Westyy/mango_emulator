using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId)
            : base(ServerPacketHeadersNew.ObjectUpdateMessageComposer)
        {
            WriteFloorItem(Item, UserId);
        }

        private void WriteFloorItem(Item Item, int UserId)
        {
            int S = 0;

            if (Item.Data.Behaviour == ItemBehaviour.MUSIC_DISK)
            {
                int.TryParse(Item.DisplayFlags, out S);
            }

            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.Data.SpriteId);
            base.WriteInteger(Item.Position.X);
            base.WriteInteger(Item.Position.Y);
            base.WriteInteger(Item.RoomRot);
            base.WriteDouble(Item.Position.Z);
            base.WriteInteger(0);
            base.WriteInteger(S); // secondary
            base.WriteString(Item.DisplayFlags);
            base.WriteInteger(Item.Expires ? (Item.Expired ? (int)((double)(Math.Ceiling(Item.ExpireTimeLeft / 60))) + 1 : 0) : 0); // todo: check
            base.WriteInteger(Item.Usable ? 1: 0);
            base.WriteInteger(UserId);
        }
    }
}
