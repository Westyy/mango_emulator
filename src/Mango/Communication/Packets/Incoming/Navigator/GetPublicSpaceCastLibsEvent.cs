using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class GetPublicSpaceCastLibsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int roomId = packet.PopWiredInt();
            RoomData data = null;

            if (!RoomLoader.TryGetData(roomId, out data))
            {
                return;
            }

            //session.SendPacket(new PublicSpaceCastLibsComposer(data));
        }
    }
}
