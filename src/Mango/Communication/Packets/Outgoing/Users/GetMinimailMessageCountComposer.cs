using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class GetMinimailMessageCountComposer : ServerPacket
    {
        public GetMinimailMessageCountComposer()
            : base(ServerPacketHeadersNew.GetMinimailMessageCountComposer)
        {
            //todo: minimail
            base.WriteInteger(0);
        }
    }
}
