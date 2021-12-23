using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Inventory.Furni;

namespace Mango.Communication.Packets.Incoming.Inventory.Furni
{
    class RequestFurniInventoryEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            ICollection<Item> FloorItems = session.GetPlayer().GetInventory().GetFloorItems();
            ICollection<Item> WallItems = session.GetPlayer().GetInventory().GetWallItems();

            session.SendPacket(new FurniListComposer("S", FloorItems));
            session.SendPacket(new FurniListComposer("I", WallItems));
        }
    }
}
