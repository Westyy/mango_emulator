using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    static class ItemBehaviourUtility
    {
        public static ItemBehaviour FromString(string BehaviourString)
        {
            switch (BehaviourString.ToLower())
            {
                case "rental":

                    return ItemBehaviour.RENTAL;

                case "musicdisk":

                    return ItemBehaviour.MUSIC_DISK;

                case "fireworks":

                    return ItemBehaviour.FIREWORKS;

                case "dispenser":

                    return ItemBehaviour.DISPENSER;

                case "pet":

                    return ItemBehaviour.PET;

                case "scoreboard":

                    return ItemBehaviour.SCOREBOARD;

                case "prizetrophy":

                    return ItemBehaviour.PRIZE_TROPHY;

                case "stickypole":

                    return ItemBehaviour.STICKY_POLE;

                case "stickynote":

                    return ItemBehaviour.STICKY_NOTE;

                case "loveshuffler":

                    return ItemBehaviour.LOVE_SHUFFLER;

                case "spinningbottle":

                    return ItemBehaviour.SPINNING_BOTTLE;

                case "habbowheel":

                    return ItemBehaviour.HABBO_WHEEL;

                case "dice":

                    return ItemBehaviour.DICE;

                case "holodice":

                    return ItemBehaviour.HOLO_DICE;

                case "football":

                    return ItemBehaviour.FOOTBALL;

                case "autoswitch":

                    return ItemBehaviour.STEP_SWITCH;

                case "alert":

                    return ItemBehaviour.TIMED_ALERT;

                case "wiredeffect":

                    return ItemBehaviour.WIRED_EFFECT;

                case "wiredcondition":

                    return ItemBehaviour.WIRED_CONDITION;

                case "wiredtrigger":

                    return ItemBehaviour.WIRED_TRIGGER;

                case "effectgenerator":

                    return ItemBehaviour.AVATAR_EFFECT_GENERATOR;

                case "roller":

                    return ItemBehaviour.ROLLER;

                case "moodlight":

                    return ItemBehaviour.MOODLIGHT;

                case "traxplayer":

                    return ItemBehaviour.TRAX_PLAYER;

                case "onewaygate":

                    return ItemBehaviour.ONE_WAY_GATE;

                case "teleporter":

                    return ItemBehaviour.TELEPORTER;

                case "gate":

                    return ItemBehaviour.GATE;

                case "exchange":

                    return ItemBehaviour.EXCHANGE_ITEM;

                case "bed":

                    return ItemBehaviour.BED;

                case "seat":

                    return ItemBehaviour.SEAT;

                case "switch":

                    return ItemBehaviour.SWITCHABLE;

                case "landscape":

                    return ItemBehaviour.LANDSCAPE;

                case "floor":

                    return ItemBehaviour.FLOOR;

                case "wallpaper":

                    return ItemBehaviour.WALLPAPER;

                default:

                    return ItemBehaviour.STATIC;
            }
        }
    }
}
