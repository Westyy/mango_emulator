using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class AboutCmd : IChatCommand
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
            TimeSpan Uptime = DateTime.Now - Mango.ServerStarted;
            Session.SendPacket(new ModMessageComposer("Mango Server " + Mango.Version + "\n\n"
                + "Uptime: " + Uptime.Days + " day(s) , " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes"));
        }
    }
}
