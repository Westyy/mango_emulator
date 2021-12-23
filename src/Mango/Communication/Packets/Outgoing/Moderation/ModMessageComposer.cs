using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModMessageComposer : ServerPacket
    {
        public ModMessageComposer(string text, string url = "")
            : base(ServerPacketHeadersNew.ModMessageComposer)
        {
            base.WriteString(text);
            base.WriteString(url);
        }
    }
}
