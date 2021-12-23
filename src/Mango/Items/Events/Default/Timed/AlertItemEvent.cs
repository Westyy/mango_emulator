using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Timed
{
    class AlertItemEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            switch (Type)
            {
                case ItemEventType.InstanceLoaded:
                case ItemEventType.Placed:
                case ItemEventType.Removing:
                    Item.Flags = "0";
                    Item.DisplayFlags = "0";
                    break;

                case ItemEventType.Interact:
                    if (Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()) || Item.Flags == "1")
                    {
                        break;
                    }

                    Item.Flags = "1";
                    Item.DisplayFlags = "1";

                    Instance.GetItems().BroadcastItemState(Item);
                    Item.RequestUpdate(4);
                    break;

                case ItemEventType.UpdateTick:
                    if (Item.Flags == "1")
                    {
                        break;
                    }

                    Item.Flags = "0";
                    Item.DisplayFlags = "0";

                    Instance.GetItems().BroadcastItemState(Item);
                    break;
            }
        }
    }
}
