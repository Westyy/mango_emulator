using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Games
{
    class Game2AccountGameStatusMessageComposer : ServerPacket
    {
        public Game2AccountGameStatusMessageComposer(Player Player)
            : base(ServerPacketHeadersNew.Game2AccountGameStatusMessageComposer)
        {
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteInteger(12);
            base.WriteInteger(10);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(5);
            base.WriteInteger(Player.Id);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteBoolean(false);
        }
    }
}
