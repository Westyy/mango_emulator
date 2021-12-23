using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Games
{
    class GameCenterInitComposer : ServerPacket
    {
        public GameCenterInitComposer(string ImgUrl)
            : base(ServerPacketHeadersNew.GameCenterInitComposer)
        {
            base.WriteInteger(1); // game count

            // loop
            base.WriteInteger(0);
            base.WriteString("snowwar");
            base.WriteString("93d4f3");
            base.WriteString("");
            base.WriteString(ImgUrl);
            base.WriteString("");
        }
    }
}
