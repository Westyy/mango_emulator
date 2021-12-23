using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class OnlineCmd : IChatCommand
    {
        public int RequiredRank
        {
            get { return -1; }
        }

        public string PermissionRequired
        {
            get { return ""; }
        }

        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, string Message)
        {
            Session.SendPacket(new ModMessageComposer("Online Players: " + Mango.GetServer().GetPlayerManager().Count));
        }
    }
}
