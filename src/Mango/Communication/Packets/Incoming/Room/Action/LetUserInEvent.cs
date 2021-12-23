using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Room.Session;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Room.Action
{
    class LetUserInEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!Session.GetPlayer().GetAvatar().InRoom || !Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()))
            {
                return;
            }

            string Username = Packet.PopString();
            bool Accepted = Packet.PopWiredBoolean();

            Player Player = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(Username, out Player))
            {
                return;
            }

            if (Player.GetAvatar().AuthOK)
            {
                return;
            }

            if (Accepted)
            {
                Player.GetAvatar().AuthOK = true;
                Player.GetSession().SendPacket(new FlatAccessibleComposer());
            }
            else
            {
                Player.GetSession().SendPacket(new FlatAccessDeniedComposer());
                Player.GetAvatar().ExitRoom(false);
            }
        }
    }
}
