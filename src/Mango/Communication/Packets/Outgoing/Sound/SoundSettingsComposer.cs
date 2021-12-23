using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Sound
{
    class SoundSettingsComposer : ServerPacket
    {
        public SoundSettingsComposer(int Volume)
            : base(ServerPacketHeadersNew.SoundSettingsComposer)
        {
            // todo: other sound settings
            if (Volume < 0)
            {
                Volume = 0;
            }

            if (Volume > 100)
            {
                Volume = 100;
            }

            base.WriteInteger(Volume); // System
            base.WriteInteger(Volume); // Furni
            base.WriteInteger(Volume); // Trax
        }
    }
}
