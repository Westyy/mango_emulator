using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Quests;

namespace Mango.Communication.Packets.Incoming.Quest
{
    class GetQuestsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            //session.SendPacket(new ModMessageComposer("Quests are currently disabled."));
            session.SendPacket(new QuestListComposer(session.GetPlayer(), Mango.GetServer().GetQuestManager().QuestList));
        }
    }
}
