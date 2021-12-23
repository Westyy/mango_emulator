using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class RotateCmd : IChatCommand
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

            RoomAvatar TargetAvatar = null;

            if (!Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().TryGet(msg, out TargetAvatar))
            {
                return;
            }

            TargetAvatar.BodyRotation = Session.GetPlayer().GetAvatar().BodyRotation;
            TargetAvatar.HeadRotation = Session.GetPlayer().GetAvatar().HeadRotation;
            TargetAvatar.UpdateNeeded = true;
        }
    }
}
