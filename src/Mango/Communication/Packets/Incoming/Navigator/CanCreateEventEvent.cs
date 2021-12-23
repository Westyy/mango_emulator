using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;
namespace Mango.Communication.Packets.Incoming.Navigator
{
    class CanCreateEventEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (session.GetPlayer().GetAvatar().InRoom)
            {
                if (instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
                {
                    /*if (session.GetPlayer().GetPermissions().AccDisableCreateRoomEvents)
                    {
                        session.SendPacket(new CanCreateRoomEventComposer(4));
                        return;
                    }*/

                    if (instance.Access != RoomAccess.Open)
                    {
                        session.SendPacket(new CanCreateRoomEventComposer(3));
                        return;
                    }

                    session.SendPacket(new CanCreateRoomEventComposer(0));
                }
            }
        }
    }
}
