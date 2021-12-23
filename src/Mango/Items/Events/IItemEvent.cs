using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events
{
    interface IItemEvent
    {
        bool IsAsynchronous { get; }
        void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData);
    }
}
