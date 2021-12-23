using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango
{
    /// <summary>
    /// Globally configurable static settings
    /// </summary>
    static class MangoStaticSettings
    {
        /// <summary>
        /// Want the user to walk up to furni on click?
        /// </summary>
        public const bool WalkUpToFurniOnClick = true;

        /// <summary>
        /// If the room owner can be kicked from his own room when going AFK.
        /// </summary>
        public const bool RoomOwnerNoAFKKick = true;

        /// <summary>
        /// Maximum favourite rooms a user can have.
        /// </summary>
        public const int MaximumFavouriteRooms = 15;

        /// <summary>
        /// Maximum wardrobe slots available in the client.
        /// </summary>
        public const int WardrobeClientMaxSlots = 10;

        /// <summary>
        /// The time the user has to wait once they flooded.
        /// </summary>
        public const int RoomFloodMuteTimeInSec = 20;

        /// <summary>
        /// Maximum floor furniture allowed in a room
        /// </summary>
        public const int MaxFloorFurniInRoom = 1000;

        /// <summary>
        /// Maximum wall furniture allowed in a room
        /// </summary>
        public const int MaxWallFurniInRoom = 1000;
    }
}
