using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Session
{
    class CantConnectComposer : ServerPacket
    {
        public CantConnectComposer(int errorCode)
            : base(ServerPacketHeadersNew.CantConnectMessageComposer)
        {
            base.WriteInteger(errorCode);
        }
    }
}
