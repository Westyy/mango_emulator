using Mango.Players.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectsComposer : ServerPacket
    {
        public AvatarEffectsComposer(ICollection<AvatarEffect> Effects)
            : base(ServerPacketHeadersNew.AvatarEffectsMessageComposer)
        {
            base.WriteInteger(Effects.Count);

            foreach (AvatarEffect Effect in Effects)
            {
                base.WriteInteger(Effect.SpriteId);
                base.WriteInteger(0); // 0 hand 1 costume
                base.WriteInteger((int)Effect.Duration);
                base.WriteInteger(Effect.Activated ? Effect.Quantity - 1 : Effect.Quantity);
                base.WriteInteger(Effect.Activated ? (int)Effect.TimeLeft : -1);
            }
        }
    }
}
