using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Generics
{
    class BedItemEvent : IItemEvent
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

                    if (!Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()))
                    {
                        return;
                    }

                    int CurrentState = 0;
                    int.TryParse(Item.Flags, out CurrentState);

                    int NewState = CurrentState + 1;

                    if (CurrentState < 0 || CurrentState >= (Item.Data.BehaviourData - 1))
                    {
                        NewState = 0;
                    }

                    if (CurrentState != NewState)
                    {
                        Item.Flags = NewState.ToString();
                        Item.DisplayFlags = Item.Flags;

                        Instance.GetItems().BroadcastItemState(Item);
                    }

                    break;
            }
        }
    }
}
