using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Achievements
{
    class AchievementLevel
    {
        private int _level;
        private int _rewardPixels;
        private int _rewardPoints;
        private int _requirement;

        public AchievementLevel(int Level, int RewardPixels, int RewardPoints, int Requirement)
        {
            this._level = Level;
            this._rewardPixels = RewardPixels;
            this._rewardPoints = RewardPoints;
            this._requirement = Requirement;
        }

        public int Level
        {
            get
            {
                return this._level;
            }
        }

        public int RewardPixels
        {
            get
            {
                return this._rewardPixels;
            }
        }

        public int RewardPoints
        {
            get
            {
                return this._rewardPoints;
            }
        }

        public int Requirement
        {
            get
            {
                return this._requirement;
            }
        }
    }
}
