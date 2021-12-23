using Mango.Communication.Packets.Outgoing.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopWiredInt();

            Session.GetPlayer().ChangeHomeRoom(RoomId);

            Session.SendPacket(new NavigatorSettingsComposer(RoomId));
        }
    }
}
