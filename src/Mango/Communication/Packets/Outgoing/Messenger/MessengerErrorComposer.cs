using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class MessengerErrorComposer : ServerPacket
    {
        public MessengerErrorComposer(int ErrorCode1, int ErrorCode2)
            : base(ServerPacketHeadersNew.MessengerErrorComposer)
        {
            base.WriteInteger(ErrorCode1);
            base.WriteInteger(ErrorCode2);
        }
    }
}
