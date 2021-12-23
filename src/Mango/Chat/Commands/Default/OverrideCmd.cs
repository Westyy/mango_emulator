using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class OverrideCmd : IChatCommand
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
            if (Session.GetPlayer().GetAvatar().Overriding)
            {
                Session.GetPlayer().GetAvatar().Overriding = false;
            }
            else
            {
                Session.GetPlayer().GetAvatar().Overriding = true;
            }
        }
    }
}
