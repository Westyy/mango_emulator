using Mango.Communication.Packets.Outgoing.Availability;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class MaintenanceCmd : IChatCommand
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
            string[] s = Message.Split(' ');

            if (s.Length == 1)
            {
                return;
            }

            string msg = CommandManager.MergeParams(s, 1);

            int i = 0;
            if (!int.TryParse(msg, out i))
            {
                return;
            }

            Mango.GetServer().GetPlayerManager().BroadcastPacket(new MaintenanceShutdownAlertComposer(i));
        }
    }
}
