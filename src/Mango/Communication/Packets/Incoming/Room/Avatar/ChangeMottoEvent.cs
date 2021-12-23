using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class ChangeMottoEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            string NewMotto = StringCharFilter.Escape(Packet.PopString().Trim());

            if (NewMotto.Length > 38)
            {
                NewMotto = NewMotto.Substring(0, 38);
            }

            if (NewMotto == Session.GetPlayer().Motto)
            {
                return;
            }

            Session.GetPlayer().ChangeMotto(NewMotto);

            Session.SendPacket(new UserChangeComposer(Session.GetPlayer(), true));

            if (Session.GetPlayer().GetAvatar().InRoom)
            {
                RoomInstance Instance = Session.GetPlayer().GetAvatar().GetCurrentRoom();

                Instance.GetAvatars().BroadcastPacket(new UserChangeComposer(Session.GetPlayer(), false));
            }
        }
    }
}
