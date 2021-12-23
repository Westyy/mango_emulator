using Mango.Rooms.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(Vector3D From, Vector3D To, int RollerId, int AvatarId, int ItemId)
            : base(ServerPacketHeadersNew.SlideObjectBundleMessageComposer)
        {
            bool IsItem = ItemId > 0;

            base.WriteInteger(From.X);
            base.WriteInteger(From.Y);
            base.WriteInteger(To.X);
            base.WriteInteger(To.Y);
            //base.WriteBoolean(IsItem);
            base.WriteInteger(IsItem ? 1 : 0);

            if (IsItem)
            {
                base.WriteInteger(ItemId);
            }
            else
            {
                base.WriteInteger(RollerId);
                base.WriteInteger(2);
                base.WriteInteger(AvatarId);
            }

            base.WriteDouble(From.Z);
            base.WriteDouble(To.Z);

            if (IsItem)
            {
                base.WriteInteger(RollerId);
            }
        }
    }
}
