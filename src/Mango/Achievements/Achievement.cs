using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Achievements
{
    class Achievement
    {
        private string _achievementGroup;
        private int _level;
        private int _progress;

        public Achievement(string AchievementGroup, int Level, int Progress)
        {
            this._achievementGroup = AchievementGroup;
            this._level = Level;
            this._progress = Progress;
        }

        public string AchievementGroup
        {
            get
            {
                return this._achievementGroup;
            }
        }

        public int Level
        {
            get
            {
                return this._level;
            }
            set
            {
                this._level = value;
            }
        }

        public int Progress
        {
            get
            {
                return this._progress;
            }
            set
            {
                this._progress = value;
            }
        }

        public string BadgeCodeForLevel
        {
            get
            {
                return this._achievementGroup + this._level;
            }
        }
    }
}
