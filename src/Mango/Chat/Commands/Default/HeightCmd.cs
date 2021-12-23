using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Rooms.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class HeightCmd : IChatCommand
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

            RoomAvatar Avatar = Session.GetPlayer().GetAvatar();
            Avatar.Position = new Vector3D(Avatar.Position.X, Avatar.Position.Y, i);
            Avatar.UpdateNeeded = true;
        }
    }
}
