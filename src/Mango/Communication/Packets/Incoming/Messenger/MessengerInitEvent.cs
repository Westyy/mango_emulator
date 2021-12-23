using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;

using Mango.Players;
using Mango.Communication.Packets.Outgoing.Messenger;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class MessengerInitEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            ICollection<MessengerFriendship> Friends = session.GetPlayer().GetMessenger().GetFriends;

            List<PlayerData> PlayerData = new List<PlayerData>();

            foreach (MessengerFriendship Friend in Friends)
            {
                PlayerData Info = Friend.Friend;

                if (Info == null)
                {
                    Info = PlayerLoader.GetDataById(Friend.UserTwoId);

                    if (Info == null)
                    {
                        continue;
                    }
                }

                PlayerData.Add(Info);
            }

            session.SendPacket(new MessengerInitComposer(PlayerData));
        }
    }
}
