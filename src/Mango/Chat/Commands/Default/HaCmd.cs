using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Packets.Outgoing.Notifications;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class HaCmd : IChatCommand
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
            msg = msg.Replace('*', '\n');

            Mango.GetServer().GetPlayerManager().BroadcastPacket(new HabboBroadcastComposer("Hotel Alert:\n\n" + msg));
        }
    }
}
