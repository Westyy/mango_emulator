using Mango.Communication.Packets.Outgoing.Room.Settings;
using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Action
{
    class RemoveAllRightsComposer : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            foreach (int UserId in new List<int>(Instance.UsersWithRights))
            {
                if (Instance.GetRights().TakeRights(UserId))
                {
                    Session.SendPacket(new FlatControllerRemovedComposer(Instance, UserId));
                }
            }
        }
    }
}
