using Mango.Database.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Quests
{
    sealed class Quest
    {
        private int _id;
        private string _category;
        private int _seriesNumber;
        private QuestType _type;
        private int _goalData;
        private string _name;
        private int _reward;
        private string _dataBit;

        public Quest(int Id, string Category, int SeriesNumber, int Type, int GoalData, string Name, int Reward, string DataBit)
        {
            this._id = Id;
            this._category = Category;
            this._seriesNumber = SeriesNumber;

            if (Type < 0 | Type > 17)
            {
                throw new DatabaseException("Type incorrect");
            }

            switch (Type)
            {
                case 0:
                    this._type = QuestType.FURNI_MOVE;
                    break;

                case 1:
                    this._type = QuestType.FURNI_ROTATE;
                    break;

                case 2:
                    this._type = QuestType.FURNI_PLACE;
                    break;

                case 3:
                    this._type = QuestType.FURNI_PICK;
                    break;

                case 4:
                    this._type = QuestType.FURNI_SWITCH;
                    break;

                case 5:
                    this._type = QuestType.FURNI_STACK;
                    break;

                case 6:
                    this._type = QuestType.FURNI_DECORATION_FLOOR;
                    break;

                case 7:
                    this._type = QuestType.FURNI_DECORATION_WALL;
                    break;

                case 8:
                    this._type = QuestType.SOCIAL_VISIT;
                    break;

                case 9:
                    this._type = QuestType.SOCIAL_CHAT;
                    break;

                case 10:
                    this._type = QuestType.SOCIAL_FRIEND;
                    break;

                case 11:
                    this._type = QuestType.SOCIAL_RESPECT;
                    break;

                case 12:
                    this._type = QuestType.SOCIAL_DANCE;
                    break;

                case 13:
                    this._type = QuestType.SOCIAL_WAVE;
                    break;

                case 14:
                    this._type = QuestType.PROFILE_CHANGE_LOOK;
                    break;

                case 15:
                    this._type = QuestType.PROFILE_CHANGE_MOTTO;
                    break;

                case 16:
                    this._type = QuestType.PROFILE_BADGE;
                    break;
                    
                case 17:
                    this._type = QuestType.EXPLORE_FIND_ITEM;
                    break;
            }

            this._goalData = GoalData;
            this._name = Name;
            this._reward = Reward;
            this._dataBit = DataBit;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Category
        {
            get { return this._category; }
            set { this._category = value; }
        }

        public int SeriesNumber
        {
            get { return this._seriesNumber; }
            set { this._seriesNumber = value; }
        }

        public QuestType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public int GoalData
        {
            get { return this._goalData; }
            set { this._goalData = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public int Reward
        {
            get { return this._reward; }
            set { this._reward = value; }
        }

        public string DataBit
        {
            get { return this._dataBit; }
            set { this._dataBit = value; }
        }
    }
}
