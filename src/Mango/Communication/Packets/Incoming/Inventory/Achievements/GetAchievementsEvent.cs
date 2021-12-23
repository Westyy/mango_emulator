using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Inventory.Achievements;

namespace Mango.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            session.SendPacket(new AchievementsComposer(session.GetPlayer(), Mango.GetServer().GetAchievementManager().AchievementList));
        }
    }
}
