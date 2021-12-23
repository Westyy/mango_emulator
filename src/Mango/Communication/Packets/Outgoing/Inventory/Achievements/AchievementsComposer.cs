using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Achievements;

namespace Mango.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementsComposer : ServerPacket
    {
        public AchievementsComposer(Player Player, ICollection<AchievementData> AchievementDatas)
            : base(ServerPacketHeadersNew.AchievementsComposer)
        {
            base.WriteInteger(AchievementDatas.Count);

            foreach (AchievementData Data in AchievementDatas)
            {
                Achievement UserAchievement = null;

                int TargetLevel = (Player.Achievements().TryGetAchievementData(Data.GroupName, out UserAchievement) ? UserAchievement.Level + 1 : 1);
                int TotalLevels = Data.Levels.Count;

                if (TargetLevel > TotalLevels)
                {
                    TargetLevel = TotalLevels;
                }

                AchievementLevel TargetLevelData = Data.Levels[TargetLevel];

                base.WriteInteger(Data.Id);                                                           // Id
                base.WriteInteger(TargetLevel);                                                   // Target level
                base.WriteString(Data.GroupName + TargetLevel);                 // Target name/desc/badge
                base.WriteInteger(TargetLevelData.Requirement);                                   // Progress req/target        
                base.WriteInteger(TargetLevelData.RewardPixels);                                   // Pixel reward       
                base.WriteInteger(TargetLevelData.RewardPoints);                                  // Point reward ?
                base.WriteInteger(UserAchievement != null ? UserAchievement.Progress : 0);                      // Current progress
                base.WriteInteger(0);
                base.WriteBoolean(UserAchievement != null ? (UserAchievement.Level >= TotalLevels) : false);  // Set 100% completed ?
                base.WriteString(Data.Category);                                // Category
                base.WriteString(string.Empty);
                base.WriteInteger(TotalLevels);                                                   // Total amount of levels 
                base.WriteInteger(0);
            }

            base.WriteString("");
        }
    }
}
