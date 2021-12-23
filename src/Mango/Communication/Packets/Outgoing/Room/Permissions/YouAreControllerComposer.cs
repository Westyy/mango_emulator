using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Permissions
{
    class YouAreControllerComposer : ServerPacket
    {
        public YouAreControllerComposer()
            : base(ServerPacketHeadersNew.YouAreControllerMessageComposer)
        {
            base.WriteInteger(0); //todo: HALP
        }
    }
}
