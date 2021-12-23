using Mango.Communication.Packets.Outgoing.Room.Action;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class ShowFriendsCmd : IChatCommand
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
            foreach (var Friend in Session.GetPlayer().GetMessenger().GetFriends)
            {
                if (Friend.Friend != null && Friend.Friend.CurrentRoomId == Session.GetPlayer().CurrentRoomId)
                {
                    Session.SendPacket(new AvatarEffectComposer(Friend.Friend.GetAvatar(), 1));
                }
            }
        }
    }
}
