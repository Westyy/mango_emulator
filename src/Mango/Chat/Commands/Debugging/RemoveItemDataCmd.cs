using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Debugging
{
    /// <summary>
    /// Command used for debugging and testing how Item Data is handled when the Item Data is cleared.
    /// 
    /// Expected: Loaded rooms the furniture will remain throughout the time the room is loaded,
    /// rooms which are loaded after the list is cleared with furni in will not load the furni that has the data missing.
    /// </summary>
    sealed class RemoveItemDataCmd : IChatCommand
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
            Mango.GetServer().GetItemDataManager().Clear();

            Session.SendPacket(new ModMessageComposer("Item Data has been cleared."));
        }
    }
}
