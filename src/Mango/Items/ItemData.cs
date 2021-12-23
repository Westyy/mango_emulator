using Mango.Database.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    sealed class ItemData
    {
        public int Id { get; set; }
        public int SpriteId { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public ItemBehaviour Behaviour { get; set; }
        public ItemStackingBehaviour StackingBehaviour { get; set; }
        public ItemWalkableMode WalkableMode { get; set; }
        public int BehaviourData { get; set; }
        public int RoomLimit { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public float Height { get; set; }
        public bool AllowRecycle { get; set; }
        public bool AllowTrade { get; set; }
        public bool AllowSell { get; set; }
        public bool AllowGift { get; set; }
        public bool AllowInventoryStack { get; set; }

        public ItemData(int Id, int SpriteId, string Name, string Type, string Behaviour, string StackingBehaviour, string WalkableMode, int BehaviourData,
            int RoomLimit, int SizeX, int SizeY, float Height, int Recycle, int Trade, int Sell, int Gift, int InventoryStack)
        {
            this.Id = Id;
            this.SpriteId = SpriteId;
            this.Name = Name;

            if (Type != "s" && Type != "i" && Type != "h" && Type != "p" && Type != "e" && Type != "r")
                throw new DatabaseException(string.Format("Expected data to be 's' or 'i' or 'h' or 'p' or 'e' or 'r' but was '{0}'.", Type));

            switch (Type)
            {
                case "s":
                    this.Type = ItemType.FLOOR;
                    break;

                case "i":
                    this.Type = ItemType.WALL;
                    break;

                case "h":
                    this.Type = ItemType.CLUB_SUBSCRIPTION;
                    break;

                case "p":
                    this.Type = ItemType.PET;
                    break;

                case "e":
                    this.Type = ItemType.EFFECT;
                    break;

                case "r":
                    this.Type = ItemType.ROBOT;
                    break;
            }

            switch (Behaviour.ToLower())
            {
                case "rental":

                    this.Behaviour = ItemBehaviour.RENTAL;
                    break;

                case "musicdisk":

                    this.Behaviour = ItemBehaviour.MUSIC_DISK;
                    break;

                case "fireworks":

                    this.Behaviour = ItemBehaviour.FIREWORKS;
                    break;

                case "dispenser":

                    this.Behaviour = ItemBehaviour.DISPENSER;
                    break;

                case "pet":

                    this.Behaviour = ItemBehaviour.PET;
                    break;

                case "scoreboard":

                    this.Behaviour = ItemBehaviour.SCOREBOARD;
                    break;

                case "prizetrophy":

                    this.Behaviour = ItemBehaviour.PRIZE_TROPHY;
                    break;

                case "stickypole":

                    this.Behaviour = ItemBehaviour.STICKY_POLE;
                    break;

                case "stickynote":

                    this.Behaviour = ItemBehaviour.STICKY_NOTE;
                    break;

                case "loveshuffler":

                    this.Behaviour = ItemBehaviour.LOVE_SHUFFLER;
                    break;

                case "spinningbottle":

                    this.Behaviour = ItemBehaviour.SPINNING_BOTTLE;
                    break;

                case "habbowheel":

                    this.Behaviour = ItemBehaviour.HABBO_WHEEL;
                    break;

                case "dice":

                    this.Behaviour = ItemBehaviour.DICE;
                    break;

                case "holodice":

                    this.Behaviour = ItemBehaviour.HOLO_DICE;
                    break;

                case "football":

                    this.Behaviour = ItemBehaviour.FOOTBALL;
                    break;

                case "autoswitch":

                    this.Behaviour = ItemBehaviour.STEP_SWITCH;
                    break;

                case "alert":

                    this.Behaviour = ItemBehaviour.TIMED_ALERT;
                    break;

                case "wiredeffect":

                    this.Behaviour = ItemBehaviour.WIRED_EFFECT;
                    break;

                case "wiredcondition":

                    this.Behaviour = ItemBehaviour.WIRED_CONDITION;
                    break;

                case "wiredtrigger":

                    this.Behaviour = ItemBehaviour.WIRED_TRIGGER;
                    break;

                case "effectgenerator":

                    this.Behaviour = ItemBehaviour.AVATAR_EFFECT_GENERATOR;
                    break;

                case "roller":

                    this.Behaviour = ItemBehaviour.ROLLER;
                    break;

                case "moodlight":

                    this.Behaviour = ItemBehaviour.MOODLIGHT;
                    break;

                case "traxplayer":

                    this.Behaviour = ItemBehaviour.TRAX_PLAYER;
                    break;

                case "onewaygate":

                    this.Behaviour = ItemBehaviour.ONE_WAY_GATE;
                    break;

                case "teleporter":

                    this.Behaviour = ItemBehaviour.TELEPORTER;
                    break;

                case "gate":

                    this.Behaviour = ItemBehaviour.GATE;
                    break;

                case "exchange":
                    this.Behaviour = ItemBehaviour.EXCHANGE_ITEM;
                    break;

                case "bed":
                    this.Behaviour = ItemBehaviour.BED;
                    break;

                case "seat":
                    this.Behaviour = ItemBehaviour.SEAT;
                    break;

                case "switch":
                    this.Behaviour = ItemBehaviour.SWITCHABLE;
                    break;

                case "landscape":
                    this.Behaviour = ItemBehaviour.LANDSCAPE;
                    break;

                case "floor":
                    this.Behaviour = ItemBehaviour.FLOOR;
                    break;

                case "wallpaper":
                    this.Behaviour = ItemBehaviour.WALLPAPER;
                    break;

                case "static":
                    this.Behaviour = ItemBehaviour.STATIC;
                    break;

                default:
                    throw new DatabaseException("Unable to set Item Behaviour");
            }

            if (StackingBehaviour != "normal" && StackingBehaviour != "terminator" && StackingBehaviour != "initiator" && StackingBehaviour != "ignore" && StackingBehaviour != "disable")
                throw new DatabaseException(string.Format("Expected data to be 'terminator' or 'initiator' or 'ignore' or 'disable' but was '{0}'.", StackingBehaviour));

            switch (StackingBehaviour)
            {
                case "normal":
                    this.StackingBehaviour = ItemStackingBehaviour.Normal;
                    break;

                case "terminator":
                    this.StackingBehaviour = ItemStackingBehaviour.Terminator;
                    break;

                case "initiator":
                    this.StackingBehaviour = ItemStackingBehaviour.Initiator;
                    break;

                case "ignore":
                    this.StackingBehaviour = ItemStackingBehaviour.Ignore;
                    break;

                case "disable":
                    this.StackingBehaviour = ItemStackingBehaviour.Initiate_And_Terminate;
                    break;
            }

            if (WalkableMode != "0" && WalkableMode != "1" && WalkableMode != "2")
                throw new DatabaseException(string.Format("Expected data to be '0' or '1' or '2' but was '{0}'.", WalkableMode));

            switch (WalkableMode)
            {
                case "0":
                    this.WalkableMode = ItemWalkableMode.Never;
                    break;

                case "1":
                    this.WalkableMode = ItemWalkableMode.Limited;
                    break;

                case "2":
                    this.WalkableMode = ItemWalkableMode.Always;
                    break;
            }
            this.BehaviourData = BehaviourData;
            this.RoomLimit = RoomLimit;
            this.SizeX = SizeX;
            this.SizeY = SizeY;
            this.Height = Height;
            this.AllowRecycle = Recycle == 1 ? true : false;
            this.AllowTrade = Trade == 1 ? true : false;
            this.AllowSell = Sell == 1 ? true : false;
            this.AllowInventoryStack = InventoryStack == 1 ? true : false;
        }
    }
}
