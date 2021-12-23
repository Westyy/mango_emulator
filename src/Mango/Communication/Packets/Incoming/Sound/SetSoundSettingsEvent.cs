using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Incoming.Sound
{
    class SetSoundSettingsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int Volume = packet.PopWiredInt();
            bool Meh = packet.PopWiredBoolean();

            if (Volume < 0)
            {
                Volume = 0;
            }

            if (Volume > 100)
            {
                Volume = 100;
            }

            session.GetPlayer().UpdateVolume(Volume);
        }
    }
}
