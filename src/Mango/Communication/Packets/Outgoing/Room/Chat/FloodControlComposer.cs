using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Chat
{
    class FloodControlComposer : ServerPacket
    {
        public FloodControlComposer(int TimeToMuteInSec)
            : base(ServerPacketHeadersNew.FloodControlMessageComposer)
        {
            base.WriteInteger(TimeToMuteInSec);
        }
    }
}
