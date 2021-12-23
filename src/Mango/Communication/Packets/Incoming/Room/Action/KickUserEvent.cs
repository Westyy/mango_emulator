using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Action
{
    class KickUserEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()))
            {
                return;
            }

            RoomAvatar Avatar = null;

            if (!Instance.GetAvatars().TryGet(Packet.PopWiredInt(), out Avatar))
            {
                return;
            }

            Instance.GetAvatars().SoftKickAvatar(Avatar, true, true);
        }
    }
}
