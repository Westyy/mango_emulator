using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementUnlockedComposer : ServerPacket
    {
        public AchievementUnlockedComposer()
            : base(ServerPacketHeadersNew.AchievementUnlockedComposer)
        {
            base.WriteInteger(1); // Id
            base.WriteInteger(1); // Achievement level
            base.WriteInteger(144); // Unknown
            base.WriteString("ACH_RespectEarned2"); // Badge just unlocked
            base.WriteInteger(10); // Ach score reward
            base.WriteInteger(10); // Pixel Reward
            base.WriteInteger(0); // Unknown
            base.WriteInteger(10); // Unknown
            base.WriteInteger(21); // Unknown
            base.WriteString("ACH_RespectEarned1"); // Badge just passed
            base.WriteString("identity"); // Category
            base.WriteBoolean(true); // Unknown
        }
    }
}
