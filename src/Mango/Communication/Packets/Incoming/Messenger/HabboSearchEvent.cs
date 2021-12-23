using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class HabboSearchEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            string SearchQuery = StringCharFilter.Escape(packet.PopString().Replace('%',' '));

            if (SearchQuery.Length < 1)
            {
                return;
            }

            if (SearchQuery.Length > 100)
            {
                return;
            }

            List<PlayerData> PlayerInfo = PlayerLoader.SearchPlayersByUsernameLike(SearchQuery + "%");

            List<PlayerData> Friends = new List<PlayerData>();
            List<PlayerData> NonFriends = new List<PlayerData>();

            foreach (PlayerData Info in PlayerInfo)
            {
                if (session.GetPlayer().GetMessenger().IsFriends(Info.Id))
                {
                    Friends.Add(Info);
                }
                else
                {
                    NonFriends.Add(Info);
                }
            }

            session.SendPacket(new HabboSearchResultComposer(Friends, NonFriends));
        }
    }
}
