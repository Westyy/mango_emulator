using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int SenderId, string Text)
            : base(ServerPacketHeadersNew.NewConsoleMessageComposer)
        {
            base.WriteInteger(SenderId);
            base.WriteString(Text);
            base.WriteInteger(0);
        }
    }
}
