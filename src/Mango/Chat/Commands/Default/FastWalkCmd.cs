using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class FastWalkCmd : IChatCommand
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
            if (Session.GetPlayer().GetAvatar().FastWalking)
            {
                Session.GetPlayer().GetAvatar().FastWalking = false;
            }
            else
            {
                Session.GetPlayer().GetAvatar().FastWalking = true;
            }
        }
    }
}
