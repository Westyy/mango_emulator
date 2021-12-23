using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Messenger
{
    static class MessengerMessageErrorsUtility
    {
        public static int GetMessageErrorPacketNum(MessengerMessageErrors error)
        {
            switch (error)
            {
                default:
                case MessengerMessageErrors.YOUR_MUTED:
                    return 4;

                case MessengerMessageErrors.YOUR_NOT_FRIENDS:
                    return 6;

                case MessengerMessageErrors.FRIEND_NOT_ONLINE:
                    return 5;

                case MessengerMessageErrors.FRIEND_MUTED:
                    return 3;
            }
        }
    }
}
