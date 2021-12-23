using Mango.Communication.Packets.Outgoing.Room.Settings;
using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Action
{
    class AssignRightsEvent : IPacketEvent
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

            int UserId = Packet.PopWiredInt();

            RoomAvatar Avatar = null;

            if (!Instance.GetAvatars().TryGet(UserId, out Avatar))
            {
                return;
            }

            if (Instance.GetRights().GiveRights(Avatar))
            {
                Avatar.Player.GetSession().SendPacket(new FlatControllerAddedComposer(Instance.Id, Avatar.Player.Id, Avatar.Player.Username));
            }
        }
    }
}
