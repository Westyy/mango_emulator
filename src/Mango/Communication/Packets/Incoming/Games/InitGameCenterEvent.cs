using Mango.Communication.Packets.Outgoing.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Games
{
    class InitGameCenterEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            Session.SendPacket(new GameCenterInitComposer("http://localhost/bh-static/game/images/games/gamecenter_snowwar/"));
        }
    }
}
