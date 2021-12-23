using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Catalog;
using Mango.Communication.Packets.Outgoing.Catalog;

namespace Mango.Communication.Packets.Incoming.Catalog
{
    class GetCatalogIndexEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new CatalogIndexComposer(session, Mango.GetServer().GetCatalogManager().GetPages()));
        }
    }
}
