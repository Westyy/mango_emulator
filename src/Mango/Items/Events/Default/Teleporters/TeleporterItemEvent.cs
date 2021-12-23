using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Rooms.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Teleporters
{
    class TeleporterItemEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            // Items > ItemTeleporterFinder.cs
        }
    }
}
