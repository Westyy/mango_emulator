using Mango.Players.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(AvatarEffect Effect)
            : base(ServerPacketHeadersNew.AvatarEffectAddedMessageComposer)
        {
            base.WriteInteger(0); // todo: effect types
            base.WriteInteger(Effect.SpriteId);
            base.WriteInteger((int)Effect.Duration);
        }
    }
}
