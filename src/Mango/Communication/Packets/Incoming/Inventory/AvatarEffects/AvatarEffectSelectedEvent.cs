using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    class AvatarEffectSelectedEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int EffectId = Packet.PopWiredInt();

            if (EffectId < 0)
            {
                EffectId = 0;
            }

            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomAvatar Avatar = Session.GetPlayer().GetAvatar();

            if (EffectId != 0 && !Session.GetPlayer().Effects().HasEffect(EffectId, true))
            {
                return;
            }

            Avatar.ApplyEffect(EffectId);
        }
    }
}
