using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Messenger
{
    static class MessengerEventTypesUtility
    {
        public static int GetEventTypePacketNum(MessengerEventTypes type)
        {
            switch (type)
            {
                case MessengerEventTypes.Event_Started:
                    return 0;

                case MessengerEventTypes.Achievement_Unlocked:
                    return 1;

                case MessengerEventTypes.Quest_Completed:
                    return 2;

                default:
                    return -1;
            }
        }
    }
}
