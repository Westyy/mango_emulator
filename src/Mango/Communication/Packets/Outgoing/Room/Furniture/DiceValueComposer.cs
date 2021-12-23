using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Furniture
{
    class DiceValueComposer : ServerPacket
    {
        public DiceValueComposer()
            : base(ServerPacketHeader.DiceValueMessageComposer)
        {
        }
    }
}
