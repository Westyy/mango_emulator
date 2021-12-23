using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Room.Chat;
using Mango.Communication.Packets.Outgoing.Room.Engine;

namespace Mango.Communication.Packets.Incoming.Room.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                session.GetPlayer().GetAvatar().ExitRoom();
                return;
            }

            //instance.SendObjects(session);

            if (!instance.GetAvatars().AddAvatarToRoom(session.GetPlayer().GetAvatar()))
            {
                session.GetPlayer().GetAvatar().ExitRoom();
                return;
            }

            instance.SendObjects(session);

            // log room entry
            session.GetPlayer().GetMessenger().SetUpdateNeeded(false);

            session.SendPacket(new RoomEntryInfoComposer(instance, instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true)));
            session.SendPacket(new RoomVisualizationSettingsComposer(instance.HideWalls, instance.WallThickness, instance.FloorThickness));

            if (session.GetPlayer().Muted)
            {
                session.SendPacket(new FloodControlComposer(session.GetPlayer().MutedSecondsLeft));
            }

            // quests ?
        }
    }
}
