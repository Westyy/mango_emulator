using Mango.Communication.Packets.Outgoing.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class RoomAlertCmd : IChatCommand
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

        public void Parse(Communication.Sessions.Session Session, string Message)
        {
            string[] s = Message.Split(' ');

            if (s.Length == 1)
            {
                return;
            }

            string msg = CommandManager.MergeParams(s, 1);

            Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new ModMessageComposer(msg));
        }
    }
}
