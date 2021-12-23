using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Catalog;
using Mango.Communication.Packets.Outgoing.Catalog;

namespace Mango.Communication.Packets.Incoming.Catalog
{
    class GetCatalogPageEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int pageId = packet.PopWiredInt();

            CatalogPage page = null;

            if (!Mango.GetServer().GetCatalogManager().TryGetPage(pageId, out page))
            {
                return; // page doesn't exist
            }

            if (page.DummyPage)
            {
                return; // this page should not be loaded
            }

            if (page.RequiredRight.Length > 0 && !session.GetPlayer().GetPermissions().HasRight(page.RequiredRight))
            {
                return;
            }

            if (!page.Visible)
            {
                return;
            }

            if (page.Caption.EndsWith("(Coming Soon)", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            session.SendPacket(new CatalogPageComposer(page));
        }
    }
}
