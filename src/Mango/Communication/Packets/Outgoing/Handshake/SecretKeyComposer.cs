using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class SecretKeyComposer : ServerPacket
    {
        public SecretKeyComposer(string key)
            : base(ServerPacketHeadersNew.SecretKeyComposer)
        {
            base.WriteString(key);
        }
    }
}
