using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class InitCryptoComposer : ServerPacket
    {
        public InitCryptoComposer(string token)
            : base(ServerPacketHeadersNew.InitCryptoMessageComposer)
        {
            base.WriteString(token);
            base.WriteBoolean(false);
        }
    }
}
