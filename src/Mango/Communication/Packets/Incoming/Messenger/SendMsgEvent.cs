using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Players;
using Mango.Players.Messenger;
using Mango.Communication.Packets.Outgoing.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int UserId = packet.PopWiredInt();
            string Text = StringCharFilter.Escape(packet.PopString());

            if (UserId < 1 || Text.Length < 1)
            {
                return;
            }

            if (Text.Length > 200)
            {
                Text = Text.Substring(0, 200);
            }

            if (session.GetPlayer().Muted)
            {
                session.SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.YOUR_MUTED, UserId));
                return;
            }

            MessengerFriendship Friend = null;

            if (!session.GetPlayer().GetMessenger().TryGet(UserId, out Friend))
            {
                session.SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.YOUR_NOT_FRIENDS, UserId));
                return;
            }

            Player TargetPlayer = Friend.Friend;

            if (TargetPlayer == null)
            {
                session.SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.FRIEND_NOT_ONLINE, UserId));
                return;
            }

            if (Friend.Friend.Muted)
            {
                session.SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.FRIEND_MUTED, UserId));
                return;
            }

            TargetPlayer.GetSession().SendPacket(new NewConsoleMessageComposer(session.GetPlayer().Id, Text));
        }
    }
}
