using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer(Player Player, int Respect)
            : base(ServerPacketHeadersNew.RespectNotificationMessageComposer)
        {
            base.WriteInteger(Player.Id);
            base.WriteInteger(Respect);
        }
    }
}
