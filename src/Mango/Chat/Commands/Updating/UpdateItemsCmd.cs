using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Updating
{
    sealed class UpdateItemsCmd : IChatCommand
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
            Mango.GetServer().GetItemDataManager().Init();

            Session.SendPacket(new ModMessageComposer("Item Data has been refreshed, new Items added should now work correctly. Any removed items won't take affect to currently loaded instances in rooms."));
        }
    }
}
