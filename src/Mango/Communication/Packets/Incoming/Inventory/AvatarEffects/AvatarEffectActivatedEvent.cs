using Mango.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Mango.Communication.Sessions;
using Mango.Players.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    class AvatarEffectActivatedEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int EffectId = Packet.PopWiredInt();

            AvatarEffect Effect = Session.GetPlayer().Effects().GetEffectNullable(EffectId, false, true);

            if (Effect == null || Session.GetPlayer().Effects().HasEffect(EffectId, true))
            {
                return;
            }

            if (Effect.Activate())
            {
                Session.SendPacket(new AvatarEffectActivatedComposer(Effect));
            }
        }
    }
}
