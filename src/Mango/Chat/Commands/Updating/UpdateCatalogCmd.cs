using Mango.Communication.Packets.Outgoing.Catalog;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Updating
{
    class UpdateCatalogCmd : IChatCommand
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
            get { return true; }
        }

        public void Parse(Session Session, string Message)
        {
            Mango.GetServer().GetCatalogManager().Init(Mango.GetServer().GetItemDataManager());
            Session.SendPacket(new ModMessageComposer("Catalog has been updated."));

            Mango.GetServer().GetPlayerManager().BroadcastPacket(new CatalogUpdatedComposer());
        }
    }
}
