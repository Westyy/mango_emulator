using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms.Avatar;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Action
{
    class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(RoomAvatar Avatar, int EffectId)
            : base(ServerPacketHeadersNew.AvatarEffectMessageComposer)
        {
            base.WriteInteger(Avatar.Data.Id);
            base.WriteInteger(EffectId);
            base.WriteInteger(0);
        }

        public AvatarEffectComposer(RoomAvatar Avatar, AvatarEffect Effect)
            : base(ServerPacketHeadersNew.AvatarEffectMessageComposer)
        {
            base.WriteInteger(Avatar.Data.Id);
            base.WriteInteger(AvatarEffectUtility.GetEffectNum(Effect));
            base.WriteInteger(0);
        }
    }
}
