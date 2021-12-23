using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Messenger;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class GetBuddyRequestsEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            List<PlayerData> PlayerData = new List<PlayerData>();

            /*using (var s = Mango.GetServer().GetDatabase().GetSessionFactory().OpenSession())
            {
                var Requests = s.CreateCriteria<MessengerFriendship>()
                    .Add(Restrictions.Eq("UserOneId", session.GetPlayer().Id))
                    .Add(Restrictions.Eq("Confirmed", false))
                    .List<MessengerFriendship>();

                foreach (var Request in Requests)
                {
                    PlayerData Info = PlayerLoader.GetInfoById(Request.UserTwoId);

                    if (Info != null)
                    {
                        PlayerData.Add(Info);
                    }
                }
            }*/

            ICollection<MessengerFriendship> Requests = Session.GetPlayer().GetMessenger().GetRequests;

            foreach (MessengerFriendship Request in Requests)
            {
                PlayerData Data = Request.Friend;

                if (Data == null)
                {
                    Data = PlayerLoader.GetDataById(Request.UserTwoId);
                }

                if (Data != null)
                {
                    PlayerData.Add(Data);
                }
            }

            Session.SendPacket(new BuddyRequestsComposer(PlayerData));
        }
    }
}
