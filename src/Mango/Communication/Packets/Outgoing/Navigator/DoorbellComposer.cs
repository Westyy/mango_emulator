using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class DoorbellComposer : ServerPacket
    {
        public DoorbellComposer(PlayerData player)
            : base(ServerPacketHeadersNew.DoorbellMessageComposer)
        {
            if (player != null)
            {
                base.WriteString(player.Username);
            }
            else
            {
                base.WriteString("");
            }
        }
    }
}
