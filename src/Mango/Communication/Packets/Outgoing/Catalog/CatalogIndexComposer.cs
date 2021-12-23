using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Catalog;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets.Outgoing.Catalog
{
    class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(Session Session, ICollection<CatalogPage> Pages, bool UseTestIndex = false, int TestIndexAmount = 130)
            : base(ServerPacketHeadersNew.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);

            if (UseTestIndex)
            {
                WriteTestIndex(TestIndexAmount);
                return;
            }

            foreach (CatalogPage page in Pages)
            {
                if (page.ParentId != -1 || (page.RequiredRight.Length > 0 && !Session.GetPlayer().GetPermissions().HasRight(page.RequiredRight)))
                {
                    continue;
                }

                WritePage(page, CalcTreeSize(Session, Pages, page.Id));

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != page.Id || (child.RequiredRight.Length > 0 && !Session.GetPlayer().GetPermissions().HasRight(child.RequiredRight)))
                    {
                        continue;
                    }

                    WritePage(child, 0);
                }
            }

            base.WriteBoolean(false);
        }

        internal void WriteRootIndex(Session Session, ICollection<CatalogPage> Pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(CalcTreeSize(Session, Pages, -1));
        }

        internal void WriteTestIndex(int IndexAmount)
        {
            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(-1); // page id
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(IndexAmount);

            for (int i = 0; i < IndexAmount; i++)
            {
                base.WriteBoolean(true);
                base.WriteInteger(i);
                base.WriteInteger(i);
                base.WriteInteger(i);
                base.WriteString("#" + i);
                base.WriteInteger(0);
            }
        }

        internal void WritePage(CatalogPage page, int TreeSize)
        {
            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Colour);
            base.WriteInteger(page.Icon);
            base.WriteInteger(page.Id);
            base.WriteString(page.Template);
            base.WriteString(page.Caption);
            base.WriteInteger(TreeSize);
        }

        internal int CalcTreeSize(Session Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;

            foreach (CatalogPage page in Pages)
            {
                if (page.RequiredRight.Length > 0 && !Session.GetPlayer().GetPermissions().HasRight(page.RequiredRight))
                {
                    continue;
                }

                if (page.ParentId == ParentId)
                {
                    i++;
                }
            }

            return i;
        }
    }
}
