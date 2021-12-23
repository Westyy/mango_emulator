using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Rooms.Mapping;
using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Randomizers
{
    class DiceItemEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return true; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            switch (Type)
            {
                case ItemEventType.Removing:
                case ItemEventType.Placed:

                    if (Item.Flags == "0" && Item.Flags != "1") // 0 = dice switched off
                    {
                        Item.Flags = "0";
                        Item.DisplayFlags = "0";
                    }

                    break;

                case ItemEventType.Interact:

                    RoomAvatar Avatar = Session.GetPlayer().GetAvatar();

                    if (!Avatar.InRoom)
                    {
                        return;
                    }

                    if (!DistanceCalculator.TilesTouching(Avatar.Position.ToVector2D(), Item.Position.ToVector2D()))
                    {
                        Avatar.MoveToAndInteractItem(Item, RequestData);
                        return;
                    }

                    if (RequestData >= 0)
                    {
                        if (Item.Flags != "-1")
                        {
                            Item.Flags = "-1";
                            Item.DisplayFlags = "-1";

                            Instance.GetItems().BroadcastItemState(Item);
                            Item.RequestUpdate(3);
                        }
                    }
                    else
                    {
                        Item.Flags = "0";
                        Item.DisplayFlags = "0";
                    }

                    break;

                case ItemEventType.UpdateTick:

                    Item.Flags = RandomNumber.GenerateRandom(1, 6).ToString();
                    Item.DisplayFlags = Item.Flags;

                    Instance.GetItems().BroadcastItemState(Item);
                    break;
            }
        }
    }
}
