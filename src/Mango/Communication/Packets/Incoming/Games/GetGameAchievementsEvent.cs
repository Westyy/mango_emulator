using Mango.Communication.Packets.Outgoing.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Games
{
    class GetGameAchievementsEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            Session.SendPacket(new GetGameAchievementComposer(Session.GetPlayer(), Mango.GetServer().GetAchievementManager().GamesAchievements));
            
            // Leaderboard
            Session.SendPacket(new GetFriendLeaderboardComposer(Session.GetPlayer()));

            Session.SendPacket(new Game2AccountGameStatusMessageComposer(Session.GetPlayer()));
        }
    }
}
