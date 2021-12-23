using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class GenericErrorComposer : ServerPacket
    {
        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeadersNew.GenericErrorComposer)
        {
            base.WriteInteger(errorId);
        }
    }
}
