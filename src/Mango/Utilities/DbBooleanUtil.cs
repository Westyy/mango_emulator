using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Utilities
{
    static class DbBooleanUtil
    {
        public static bool GetBooleanFromString(string b)
        {
            switch (b.ToLower())
            {
                case "1":
                    return true;

                case "0":
                    return false;

                case "y":
                    return true;

                case "n":
                    return false;

                default:
                    throw new ArgumentException("Unable to determine the string provided as a Boolean. Input was: " + b);
            }
        }
    }
}
