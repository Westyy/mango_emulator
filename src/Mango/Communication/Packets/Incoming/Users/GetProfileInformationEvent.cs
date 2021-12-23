using Mango.Communication.Packets.Outgoing.Users;
using Mango.Communication.Sessions;
using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Users
{
    class GetProfileInformationEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int UserId = Packet.PopWiredInt();
            bool IsMe = Packet.PopWiredBoolean();

            PlayerData Data = null;
            Player Player = null;

            if (Mango.GetServer().GetPlayerManager().TryGet(UserId, out Player))
            {
                Data = Player;
            }
            else
            {
                Data = PlayerLoader.GetDataById(UserId);
            }

            if (Data == null)
            {
                return;
            }

            Session.SendPacket(new ProfileInformationComposer(Session.GetPlayer(), Data, Player != null ? Player : null));
        }
    }
}
