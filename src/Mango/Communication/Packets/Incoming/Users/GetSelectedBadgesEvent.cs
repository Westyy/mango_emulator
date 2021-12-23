using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Users;

namespace Mango.Communication.Packets.Incoming.Users
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            int RequestedPlayerId = packet.PopWiredInt();
            Player TargetPlayer = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(RequestedPlayerId, out TargetPlayer))
            {
                return;
            }

            session.SendPacket(new HabboUserBadgesComposer(TargetPlayer, TargetPlayer.GetBadges().EquippedBadges));
        }
    }
}
