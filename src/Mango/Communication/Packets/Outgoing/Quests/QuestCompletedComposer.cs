using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Quests
{
    class QuestCompletedComposer:ServerPacket
    {
        public QuestCompletedComposer()
            : base(ServerPacketHeadersNew.QuestCompletedComposer)
        {
            base.WriteString("Category");
            base.WriteInteger(1); // Like order number i guess
            base.WriteInteger(2); // Amount in the category
            base.WriteInteger(3); // Reward Type
            base.WriteInteger(1);
            base.WriteBoolean(false); // Already started
            base.WriteString("find_someone"); // Action name
            base.WriteString(""); // Data bit?
            base.WriteInteger(10); // Reward
            base.WriteString(""); // Name
            base.WriteInteger(2); // User progress
            base.WriteInteger(10); // Goal progress
            base.WriteInteger(0); // Unlock in... (in seconds)
            base.WriteString("");
            base.WriteString("");
            base.WriteBoolean(true);
            base.WriteBoolean(true); // Activate next quest window
        }
    }
}
