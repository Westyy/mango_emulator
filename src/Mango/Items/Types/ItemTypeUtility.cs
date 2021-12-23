using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items
{
    static class ItemTypeUtility
    {
        public static string GetTypeLetter(ItemType type)
        {
            switch (type)
            {
                default:
                case ItemType.FLOOR:
                    return "s";

                case ItemType.WALL:
                    return "i";

                case ItemType.CLUB_SUBSCRIPTION:
                    return "h";

                case ItemType.PET:
                    return "p";

                case ItemType.EFFECT:
                    return "e";
            }
        }
    }
}
