using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Advertisement
{
    class InterstitialComposer : ServerPacket
    {
        public InterstitialComposer()
            : base(ServerPacketHeadersNew.InterstitialMessageComposer)
        {
            base.WriteString(string.Empty);
            base.WriteString(string.Empty);
        }
    }
}
