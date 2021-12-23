using Mango.Communication.Packets.Outgoing.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class CommandsCmd : IChatCommand
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

        public void Parse(Communication.Sessions.Session Session, string Message)
        {
            Session.SendPacket(new ModMessageComposer("You can use: ':about, :online'"));
        }
    }
}
