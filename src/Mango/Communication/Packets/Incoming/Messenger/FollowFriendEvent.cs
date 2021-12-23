using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Room.Session;
using Mango.Communication.Packets.Outgoing.Moderation;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class FollowFriendEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int UserId = packet.PopWiredInt();

            if (!session.GetPlayer().GetMessenger().IsFriends(UserId))
            {
                return;
            }

            Player Player = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(UserId, out Player))
            {
                return;
            }

            if (!Player.GetAvatar().InRoom)
            {
                return;
            }

            if (session.GetPlayer().GetAvatar().InRoom && Player.GetAvatar().GetCurrentRoom().Id == session.GetPlayer().GetAvatar().GetCurrentRoom().Id)
            {
                return;
            }

            session.SendPacket(new RoomForwardComposer(Player.GetAvatar().GetCurrentRoom()));
        }
    }
}
