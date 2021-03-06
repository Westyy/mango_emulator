using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing
{
    class ServerPacketHeadersNew
    {
        public const int SecretKeyComposer = 1989;//3913;
        public const int InitCryptoMessageComposer = 1030;//216;
        public const int SessionParamsMessageComposer = 2621; // ?
        public const int UserRightsMessageComposer = 901;//1703;
        public const int AuthenticationOKMessageComposer = 347;//427;
        public const int GenericErrorComposer = 1809;//2139;
        public const int AvatarEffectsMessageComposer = 3798;//3415;
        public const int ScrSendUserInfoComposer = 3457;//3825;
        public const int UserPerksComposer = 62;//1858;
        public const int GetMinimailMessageCountComposer = 417;//1307;
        public const int UserObjectComposer = 2427;//1364;
        public const int CreditBalanceComposer = 3464;//1275;
        public const int ActivityPointsMessageComposer = 3995;//3334;
        public const int MOTDNotificationComposer = 2889;//884;
        public const int CanCreateRoomComposer = 3648;//3837;
        public const int DoorbellMessageComposer = 3473;//1609;
        public const int FavouriteChangedComposer = 1783;//1370;
        public const int FavouritesComposer = 1767;//3648;
        public const int FlatAccessDeniedMessageComposer = 3556;//1333;
        public const int FlatCreatedComposer = 2283;//3188;
        public const int GetGuestRoomResultComposer = 423;//1907;
        public const int GuestRoomSearchResultComposer = 3838;//973;
        public const int NavigatorSettingsComposer = 3001;//541;
        public const int OfficialRoomsComposer = 390;//3741;
        public const int UserFlatCatsComposer = 3063;//2465;
        public const int RoomRatingComposer = 3005;//1069;
        public const int RoomInfoUpdatedComposer = 1962;//2150;
        public const int PopularRoomTagsResultComposer = 2996;//2335;
        public const int SoundSettingsComposer = 3840;//1287;
        public const int HabboGroupBadgesMessageComposer = 477;//1829;
        public const int HabboUserBadgesMessageComposer = 31;//2385;
        public const int RespectNotificationMessageComposer = 3098;//2707;
        public const int InterstitialMessageComposer = 1760;//3693;
        public const int RoomAdMessageComposer = 1833;//91;
        public const int AvailabilityStatusMessageComposer = 3590;//2311;
        public const int WardrobeMessageComposer = 658;//2428;
        public const int BuddyRequestsComposer = 2546;//2503;
        public const int MessengerInitComposer = 109;//366;
        public const int FindFriendsProcessResultComposer= 439;//2292;
        public const int CatalogIndexMessageComposer = 957;//2971;
        public const int CatalogPageMessageComposer = 2416;//3121;
        public const int PurchaseErrorMessageComposer = 3671;//2199;
        public const int PurchaseOKMessageComposer = 2082;//145;
        public const int VoucherRedeemErrorMessageComposer = 2162;//3744;
        public const int FollowFriendFailedComposer = 946;//1144;
        public const int FriendListUpdateComposer = 1310;//3830;
        public const int FriendNotificationComposer = 1879;//1628;
        public const int HabboSearchResultComposer = 1959;//1487;
        public const int InstantMessageErrorComposer = 1424;//3431;
        public const int MessengerErrorComposer = 590;//1347;
        public const int NewBuddyRequestComposer = 3399;//709;
        public const int NewConsoleMessageComposer = 600;//682;
        public const int RoomInviteComposer = 2819;//2731;
        public const int AchievementsComposer = 852;//1090;
        public const int AchievementUnlockedComposer = 2647;//268;
        public const int AvatarEffectActivatedMessageComposer = 393;//131;
        public const int AvatarEffectAddedMessageComposer = 2024;//2423;
        public const int AvatarEffectExpiredMessageComposer = 3654;//2800;
        public const int BadgesComposer = 2999;//1855;
        public const int ModeratorInitMessageComposer = 2801;//3595;
        public const int ModeratorRoomInfoComposer = 1622;//2845;
        public const int ModeratorUserInfoComposer = 2399;//876;
        public const int ModeratorRoomChatlogComposer = 3110;//1267;
        public const int ModeratorUserChatlogComposer = 736;//282;
        public const int ModeratorIssueChatlogComposer = 413;//3059;
        public const int ModeratorUserRoomVisitsComposer = 3652;//3002;
        public const int ModMessageComposer = 2131;//2426;
        public const int OpenHelpToolComposer = 3155;//3696;
        public const int AvatarEffectMessageComposer = 2649;//3323;
        public const int CarryObjectMessageComposer = 3684;//1888;
        public const int DanceMessageComposer = 244;//1918;
        public const int SleepMessageComposer = 3131;//857;
        public const int WaveMessageComposer = 3872;//2493;
        public const int ChatMessageComposer = 2655;//1513;
        public const int FloodControlMessageComposer = 1804;//1208;
        public const int ShoutMessageComposer = 1152;//3609;
        public const int WhisperMessageComposer = 2832;//493;
        public const int UserTypingMessageComposer = 1552;//793;
        public const int FloorHeightMapMessageComposer = 2606;//147;
        public const int FurnitureAliasesMessageComposer = 2004;//2130;
        public const int HeightMapMessageComposer = 3071;//17;
        public const int HeightMapUpdateMessageComposer = 1820;//308;
        public const int ItemAddMessageComposer = 3029;//1337;
        public const int ItemDataUpdateMessageComposer = 2313;//3327;
        public const int ItemRemoveMessageComposer = 3467;//2370;
        public const int ItemsMessageComposer = 2862;//2262;
        public const int ItemUpdateMessageComposer = 2602;//161;
        public const int CantConnectMessageComposer = 3102;//1127;
        public const int CloseConnectionMessageComposer = 1620;//3760;
        public const int FlatAccessibleMessageComposer = 3082;//116;
        public const int OpenConnectionMessageComposer = 620;//769;
        public const int RoomForwardMessageComposer = 1175;//1676;
        public const int RoomQueueStatusMessageComposer = 313;//3516;
        public const int RoomReadyMessageComposer = 22;//3072;
        public const int ObjectAddMessageComposer = 2461;//1614;
        public const int ObjectDataUpdateMessageComposer = 582;//1787;
        public const int ObjectRemoveMessageComposer = 1705;//1393;
        public const int ObjectsMessageComposer = 2616;//2540;
        public const int PlaceObjectErrorMessageComposer = 307;//1196;
        public const int RoomEntryInfoMessageComposer = 1024;//1684;
        public const int RoomPropertyMessageComposer = 1186;//647;
        public const int RoomVisualizationSettingsComposer = 611;//1095;
        public const int UserChangeMessageComposer = 3341;//1484;
        public const int UserRemoveMessageComposer = 1173;//3882;
        public const int UserUpdateMessageComposer = 2397;//987;
        public const int UsersMessageComposer = 2330;//2756;
        public const int ObjectUpdateMessageComposer = 2290;//853;
        public const int RoomSettingsDataComposer = 2443;//3475;
        public const int RoomSettingsSavedComposer = 1494;//317;
        public const int FlatControllerAddedComposer = 1180;//175;
        public const int FlatControllerRemovedComposer = 1499;//1157;
        public const int YouAreOwnerMessageComposer = 2207;//633;
        public const int YouAreControllerMessageComposer = 2950;//532;
        public const int YouAreNotControllerMessageComposer = 1088;//2452;
        public const int FurniListComposer = 2963;//1812;
        public const int FurniListInsertComposer = 231;//1123;
        public const int FurniListRemoveComposer = 702;//231;
        public const int FurniListUpdateComposer = 57;//2810;
        public const int ActivateQuestComposer = 643;//2605;
        public const int QuestListComposer = 3218;//2872;
        public const int QuestAbortedComposer = 825;//2929;
        public const int QuestCompletedComposer = 1754;//75;
        public const int ModeratorSupportTicketComposer = 3662;//415;
        public const int HabboActivityPointNotificationMessageComposer = 927;//940;
        public const int HabboBroadcastMessageComposer = 3578;//3808;
        public const int ProfileInformationComposer = 2319;//2814;
        public const int MaintenanceShutdownAlert = 2519;

        public const int GameCenterInitComposer = 1326;
        public const int GetGameAchievementComposer = 28;
        public const int GetFriendLeaderboardComposer = 919;
        public const int Game2AccountGameStatusMessageComposer = 2277;

        public const int HabboClubOffersMessageComposer = 3338;
        public const int HabboClubExtendComposer = 1264;

        public const int SlideObjectBundleMessageComposer = 1406;

        public const int CatalogUpdatedComposer = 1528;

    }
}
