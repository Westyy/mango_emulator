using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Navigator
{
    static class NavigatorOfficialDisplayTypeUtility
    {
        public static int GetOfficialDisplayTypeNum(NavigatorOfficialDisplayType type)
        {
            switch (type)
            {
                default:
                case NavigatorOfficialDisplayType.BANNER:
                    return 0;

                case NavigatorOfficialDisplayType.DETAILED:
                    return 1;
            }
        }
    }
}
