using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Wired
{
    class WiredTriggerEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            switch (Type)
            {
                case ItemEventType.Interact:
                    {

                    }
                    break;

                case ItemEventType.InstanceLoaded:
                    {

                    }
                    break;
            }
        }
    }
}
