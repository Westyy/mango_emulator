using Mango.Communication.Packets.Outgoing.Catalog;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    /// <summary>
    /// Used just for testing..
    /// </summary>
    class TestCmd : IChatCommand
    {
        public int RequiredRank
        {
            get { return -1; }
        }

        public string PermissionRequired
        {
            get { return "mod_tool"; }
        }

        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, string Message)
        {
            Session.SendPacket(new HabboClubExtendComposer(Mango.GetServer().GetCatalogManager().FirstOffer(), 0));
        }
    }
}
