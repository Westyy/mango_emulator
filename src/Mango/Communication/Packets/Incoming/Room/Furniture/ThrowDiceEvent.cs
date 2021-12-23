using Mango.Communication.Sessions;
using Mango.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Furniture
{
    class ThrowDiceEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            Item Item = null;

            if (!Session.GetPlayer().GetAvatar().GetCurrentRoom().GetItems().TryGetItem(Packet.PopWiredInt(), out Item))
            {
                return;
            }

            if (Item.Data.Behaviour == ItemBehaviour.STATIC)
            {
                return;
            }

            int RequestData = Packet.PopWiredInt();

            Mango.GetServer().GetItemEventManager().Handle(Session, Item, Items.Events.ItemEventType.Interact, Session.GetPlayer().GetAvatar().GetCurrentRoom(), RequestData);
        }
    }
}
