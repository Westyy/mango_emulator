using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Sessions;
using Mango.Items;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat.Commands.Default
{
    class HideFurniCmd : IChatCommand
    {
        public int RequiredRank
        {
            get { return -1; }
        }

        public string PermissionRequired
        {
            get { return "mod_tool"; }
        }

        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, string Message)
        {
            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            foreach (Item Item in Instance.GetItems().GetWallAndFloor)
            {
                if (Item.Data.Type == ItemType.FLOOR)
                {
                    Instance.GetAvatars().BroadcastPacket(new ObjectRemoveComposer(Item, Session.GetPlayer().Id));
                }
                else if (Item.Data.Type == ItemType.WALL)
                {
                    Instance.GetAvatars().BroadcastPacket(new ItemRemoveComposer(Item, Session.GetPlayer().Id));
                }
            }
        }
    }
}
