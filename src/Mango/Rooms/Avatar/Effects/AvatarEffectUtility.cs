using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Avatar
{
    static class AvatarEffectUtility
    {
        public static int GetEffectNum(AvatarEffect effect)
        {
            switch (effect)
            {
                default:
                    return 0;

                case AvatarEffect.GLOW:
                    return 1;

                case AvatarEffect.BLUE_SKATEBOARD:
                    return 2;

                case AvatarEffect.BLUE_CAR:
                    return 3;

                case AvatarEffect.SPARKLE:
                    return 4;

                case AvatarEffect.TORCH:
                    return 5;

                case AvatarEffect.ROCKET:
                    return 6;

                case AvatarEffect.BUTTERFLIES:
                    return 7;

                case AvatarEffect.FIREFLIES:
                    return 8;

                case AvatarEffect.LOVE_HEARTS:
                    return 9;

                case AvatarEffect.FLIES:
                    return 10;

                case AvatarEffect.GREEN_GLOW:
                    return 11;

                case AvatarEffect.FROZEN:
                    return 12;

                case AvatarEffect.GHOST:
                    return 13;

                case AvatarEffect.PINK_SKATEBOARD:
                    return 14;

                case AvatarEffect.YELLOW_SKATEBOARD:
                    return 15;

                case AvatarEffect.MICROPHONE:
                    return 16;

                case AvatarEffect.PINK_CAR:
                    return 17;

                case AvatarEffect.YELLOW_CAR:
                    return 18;

                case AvatarEffect.POLICE_CAR:
                    return 19;

                case AvatarEffect.AMBULANCE_CAR:
                    return 20;

                case AvatarEffect.BLUE_SPORTS_CAR:
                    return 21;

                case AvatarEffect.YELLOW_SPORTS_CAR:
                    return 22;

                case AvatarEffect.FLOATING_GREEN_GLOW:
                    return 23;

                case AvatarEffect.RAINING:
                    return 24;

                case AvatarEffect.FIRE:
                    return 25;

                case AvatarEffect.FIRE_STAFF:
                    return 26;

                case AvatarEffect.KNIGHT_HELMET_WITH_SWORD:
                    return 27;

                case AvatarEffect.SWIMMING_IN_WATER:
                    return 28;

                case AvatarEffect.SWIMMING_IN_WATER2:
                    return 29;

                case AvatarEffect.PADDLING_IN_WATER:
                    return 30;

                case AvatarEffect.TIGER:
                    return 31;

                case AvatarEffect.DESPICABLE_ME_MINION_EYE:
                    return 32;

                case AvatarEffect.BANZAI_PURPLE:
                    return 33;

                case AvatarEffect.BANZAI_GREEN:
                    return 34;

                case AvatarEffect.BANZAI_BLUE:
                    return 35;

                case AvatarEffect.BANZAI_YELLOW:
                    return 36;

                case AvatarEffect.PADDLING_GREEN_WATER:
                    return 37;

                case AvatarEffect.MALE_ICE_SKATES:
                    return 38;

                case AvatarEffect.FEMALE_ICE_SKATES:
                    return 39;

                case AvatarEffect.FREEZE_GAME_RED_HELMET:
                    return 40;

                case AvatarEffect.FREEZE_GAME_GREEN_HELMET:
                    return 41;

                case AvatarEffect.FREEZE_GAME_WHITE_HELMET:
                    return 42;

                case AvatarEffect.FREEZE_GAME_BLACK_HELMET:
                    return 43;

                case AvatarEffect.SIMS_DIAMOND:
                    return 44;

                case AvatarEffect.FLOATING_SPARKLING_GLOW:
                    return 45;

                case AvatarEffect.FLOATING_SPARKLING_GLOW2:
                    return 46;

                case AvatarEffect.MEGAMIND_BLUE_FACE:
                    return 47;

                case AvatarEffect.DOG_CAR:
                    return 48;

                case AvatarEffect.FREEZE_GAME_RED_HELMET_GLOWING:
                    return 49;

                case AvatarEffect.FREEZE_GAME_GREEN_HELMET_GLOWING:
                    return 50;

                case AvatarEffect.FREEZE_GAME_WHITE_HELMET_GLOWING:
                    return 51;

                case AvatarEffect.FREEZE_GAME_BLACK_HELMET_GLOWING:
                    return 52;

                case AvatarEffect.BIRDIES_OVER_HEAD:
                    return 53;

                case AvatarEffect.BUNNY_CAR:
                    return 54;

                case AvatarEffect.BLUE_MALE_ROLLER_SKATES:
                    return 55;

                case AvatarEffect.PINK_FEMALE_ROLLER_SKATES:
                    return 56;

                case AvatarEffect.SLOWLY_GLOWING:
                    return 57;

                case AvatarEffect.SLOWLY_GLOWING2:
                    return 58;

                case AvatarEffect.PULSATING_GLOW:
                    return 59;

                case AvatarEffect.CROCODILE_HEAD:
                    return 60;

                case AvatarEffect.BIG_YELLOW_STARS_OVER_HEAD:
                    return 61;

                case AvatarEffect.BIG_PURPLE_STARS_OVER_HEAD:
                    return 62;

                case AvatarEffect.BIG_PINK_STARS_OVER_HEAD:
                    return 63;

                case AvatarEffect.CANDLE:
                    return 64;

                case AvatarEffect.MOBILE_PHONE_RINGING:
                    return 65;

                case AvatarEffect.RED_GLOWING_WAND:
                    return 66;

                case AvatarEffect.RIO_BIRD:
                    return 67;

                case AvatarEffect.BUNNY_RUN:
                    return 68;

                case AvatarEffect.RED_VROOM_CAR:
                    return 69;

                case AvatarEffect.PANDA:
                    return 70;

                case AvatarEffect.BLACK_SKATEBOARD:
                    return 71;

                case AvatarEffect.BLACK_SKATEBOARD2:
                    return 72;

                case AvatarEffect.RED_LOLLIPOP_HEAD:
                    return 73;

                case AvatarEffect.COLOUR_FLOWERS_AROUND_NECK:
                    return 74;

                case AvatarEffect.COLOUR_FLOWERS_AROUND_NECK2:
                    return 75;

                case AvatarEffect.COLOUR_FLOWERS_AROUND_NECK3:
                    return 76;

                case AvatarEffect.HORSE_SADDLE:
                    return 77;

                case AvatarEffect.PERRI:
                    return 78;

                case AvatarEffect.PURPLE_ALIEN:
                    return 79;

                case AvatarEffect.HALLOWEEN_MOUTH_FACE:
                    return 80;

                case AvatarEffect.HALLOWEEN_GREEN_HEAD:
                    return 81;

                case AvatarEffect.HALLOWEEN_MASK:
                    return 82;

                case AvatarEffect.HALLOWEEN_MASK2:
                    return 83;

                case AvatarEffect.HALLOWEEN_FACE_MUMMY:
                    return 84;

                case AvatarEffect.PUMPKIN_HEAD:
                    return 85;

                case AvatarEffect.POTATO_SACK_MASK:
                    return 86;

                case AvatarEffect.WAREWOLF_MASK:
                    return 87;

                case AvatarEffect.HALLOWEEN_GREEN_MASK:
                    return 88;

                case AvatarEffect.HALLOWEEN_KNIFE_THROUGH_BODY:
                    return 89;

                case AvatarEffect.BUTTERFLY_WINGS:
                    return 90;

                case AvatarEffect.GHOSTLY_BUTTERFLY_WINGS:
                    return 91;

                case AvatarEffect.GHOST_COVER:
                    return 92;
            }
        }
    }
}
