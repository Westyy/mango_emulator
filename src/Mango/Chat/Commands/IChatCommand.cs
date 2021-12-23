using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands
{
    interface IChatCommand
    {
        int RequiredRank { get; }
        string PermissionRequired { get; }
        bool IsAsynchronous { get; }
        void Parse(Session Session, string Message);
    }
}
