using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Achievements
{
    sealed class AchievementData
    {
        private int _id;
        private string _groupName;
        private string _category;
        private Dictionary<int, AchievementLevel> _levels;

        public AchievementData(int Id, string GroupName, string Category)
        {
            this._id = Id;
            this._groupName = GroupName;
            this._category = Category;
            this._levels = new Dictionary<int, AchievementLevel>();
        }

        public int Id
        {
            get
            {
                return this._id;
            }
        }

        public string GroupName
        {
            get
            {
                return this._groupName;
            }
        }

        public string Category
        {
            get
            {
                return this._category;
            }
        }

        public Dictionary<int, AchievementLevel> Levels
        {
            get
            {
                return this._levels;
            }
        }

        public void StoreLevel(AchievementLevel Level)
        {
            this._levels.Add(Level.Level, Level);
        }
    }
}
