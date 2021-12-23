using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Room.Engine;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class GetFurnitureAliasesEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new FurnitureAliasesComposer());
        }
    }
}
