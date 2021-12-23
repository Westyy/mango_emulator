using Mango.Players.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectActivatedComposer : ServerPacket
    {
        public AvatarEffectActivatedComposer(AvatarEffect Effect)
            : base(ServerPacketHeadersNew.AvatarEffectActivatedMessageComposer)
        {
            base.WriteInteger(Effect.SpriteId);
            base.WriteInteger((int)Effect.Duration);
        }
    }
}
