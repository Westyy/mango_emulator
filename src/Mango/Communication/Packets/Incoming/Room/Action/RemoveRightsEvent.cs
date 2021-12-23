using Mango.Communication.Packets.Outgoing.Room.Settings;
using Mango.Communication.Sessions;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Action
{
    class RemoveRightsEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            int Amount = Packet.PopWiredInt();

            for (int i = 0; (i < Amount && i <= 100); i++)
            {
                int UserId = Packet.PopWiredInt();

                if (UserId > 0 && Instance.GetRights().TakeRights(UserId))
                {
                    Session.SendPacket(new FlatControllerRemovedComposer(Instance, UserId));
                }
            }
        }
    }
}
