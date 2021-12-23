using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    static class ItemPlacementErrorUtility
    {
        public static int GetPlacementErrorNum(ItemPlacementError error)
        {
            switch (error)
            {
                default:
                case ItemPlacementError.INSUFFICIENT_RIGHTS:
                    return 1;

                case ItemPlacementError.FURNI_LIMIT_REACHED:
                    return 20;

                case ItemPlacementError.PET_LIMIT_REACHED:
                    return 21;

                case ItemPlacementError.ROLLER_LIMIT_REACHED:
                    return 22;

                case ItemPlacementError.CANNOT_PLACE_MUSIC_PLAYER:
                    return 23;
            }
        }
    }
}
