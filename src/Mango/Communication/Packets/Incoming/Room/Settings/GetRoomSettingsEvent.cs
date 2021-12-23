using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Room.Settings;

namespace Mango.Communication.Packets.Incoming.Room.Settings
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance Instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (session.GetPlayer().GetAvatar().InRoom && Instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
            {
                List<PlayerData> UsersWithRights = new List<PlayerData>();
                foreach (int UserId in new List<int>(Instance.UsersWithRights))
                {
                    PlayerData player = PlayerLoader.GetDataById(UserId);

                    if (player != null)
                    {
                        UsersWithRights.Add(player);
                    }
                }

                session.SendPacket(new RoomSettingsDataComposer(Instance, UsersWithRights));
            }
        }
    }
}
