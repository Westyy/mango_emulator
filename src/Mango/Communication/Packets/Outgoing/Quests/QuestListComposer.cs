using Mango.Players;
using Mango.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Quests
{
    class QuestListComposer : ServerPacket
    {
        public QuestListComposer(Player Player, ICollection<Quest> QuestList)
            : base (ServerPacketHeadersNew.QuestListComposer)
        {
            base.WriteInteger(QuestList.Count); // Count

            foreach (Quest Quest in QuestList)
            {
                base.WriteString(Quest.Category);
                base.WriteInteger(Quest.SeriesNumber); // Like order number i guess
                base.WriteInteger(GetCategoryCount(Quest.Category)); // Amount in the category
                base.WriteInteger(3); // Reward Type
                base.WriteInteger(Quest.Id); // Quest Id
                base.WriteBoolean(false); // Already started
                base.WriteString("find_someone"); // Action name
                base.WriteString(Quest.DataBit); // Data bit?
                base.WriteInteger(Quest.Reward); // Reward
                base.WriteString(Quest.Name); // Name
                base.WriteInteger(2); // User progress
                base.WriteInteger(Quest.GoalData); // Goal progress
                base.WriteInteger(0); // Unlock in... (in seconds)
                base.WriteString("");
                base.WriteString("");
                base.WriteBoolean(true);
            }
            base.WriteBoolean(true);
        }

        private int GetCategoryCount(string Category)
        {
            // todo: this
            return 1;
        }
    }
}
