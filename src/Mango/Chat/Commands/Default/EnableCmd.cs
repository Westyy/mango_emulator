using Mango.Communication.Packets.Outgoing.Room.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class EnableCmd : IChatCommand
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

        public void Parse(Communication.Sessions.Session Session, string Message)
        {
            int Id = Session.GetPlayer().Id;

            string[] msg = Message.Split(' ');

            if (msg.Length != 2)
            {
                return;
            }

            int EffectId = 0;

            if (!int.TryParse(msg[1], out EffectId))
            {
                return;
            }

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
            {
                return;
            }

            Session.GetPlayer().GetAvatar().ApplyEffect(EffectId);
            Session.GetPlayer().GetAvatar().UserSetEffectId = EffectId;
        }
    }
}
