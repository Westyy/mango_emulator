using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class PullCmd : IChatCommand
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
            string[] s = Message.Split(' ');

            if (s.Length == 1)
            {
                return;
            }

            string msg = CommandManager.MergeParams(s, 1);

            RoomAvatar TargetAvatar = null;

            if (!Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().TryGet(msg, out TargetAvatar))
            {
                return;
            }

            TargetAvatar.MoveTo(Session.GetPlayer().GetAvatar().SquareInFront);
        }
    }
}
