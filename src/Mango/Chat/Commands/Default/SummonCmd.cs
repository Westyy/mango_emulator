using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class SummonCmd : IChatCommand
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

            Player Player = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(msg, out Player))
            {
                Session.SendPacket(new ModMessageComposer("Player is not online."));
                return;
            }

            if (Player.GetAvatar().CurrentRoomId == Session.GetPlayer().GetAvatar().CurrentRoomId)
            {
                Session.SendPacket(new ModMessageComposer("Player is already in the room with you!"));
                return;
            }

            Player.GetAvatar().PrepareRoom(Session.GetPlayer().GetAvatar().CurrentRoomId, string.Empty, true);
        }
    }
}
