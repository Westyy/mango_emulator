using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Sound;

namespace Mango.Communication.Packets.Incoming.Sound
{
    class GetSoundSettingsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new SoundSettingsComposer(session.GetPlayer().ClientVolume));
        }
    }
}
