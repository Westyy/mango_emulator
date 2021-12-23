using Mango.Communication.Packets.Outgoing.Room.Action;
using Mango.Communication.Packets.Outgoing.Users;
using Mango.Communication.Sessions;
using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Users
{
    class RespectUserEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (Session.GetPlayer().RespectPointsLeftPlayer <= 0)
            {
                return;
            }

            int TargetPlayerId = Packet.PopWiredInt();
            Player Player = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(TargetPlayerId, out Player))
            {
                return;
            }

            Player.IncreaseRespect();
            Session.GetPlayer().DecreaseRespectToGivePlayer();

            Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new RespectNotificationComposer(Player, Player.RespectPoints));
            Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new WaveComposer(Session.GetPlayer().GetAvatar(), 7));
        }
    }
}
