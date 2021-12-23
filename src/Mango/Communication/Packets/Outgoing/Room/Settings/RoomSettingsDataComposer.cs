using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Room.Settings
{
    class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(RoomData data, ICollection<PlayerData> usersWithRights)
            : base(ServerPacketHeadersNew.RoomSettingsDataComposer)
        {
            base.WriteInteger(data.Id);
            base.WriteString(data.Name);
            base.WriteString(data.Description);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(data.Access));
            base.WriteInteger(data.CategoryId);
            base.WriteInteger(data.MaxUsers);
            base.WriteInteger(data.Model.MaxUsers);
            base.WriteInteger(data.Tags.Count);

            foreach (string tag in data.Tags)
            {
                base.WriteString(tag);
            }

            /*base.WriteInteger(usersWithRights.Count);

            foreach (PlayerData player in usersWithRights)
            {
                base.WriteInteger(player.Id);
                base.WriteString(player.Username);
            }

            base.WriteInteger(usersWithRights.Count);*/
            base.WriteInteger(0);
            base.WriteInteger(data.AllowPets ? 1 : 0);
            base.WriteInteger(data.AllowPetsEating ? 1 : 0);
            base.WriteInteger(data.DisableRoomBlocking ? 1 : 0);
            base.WriteInteger(data.HideWalls ? 1 : 0);
            base.WriteInteger(data.WallThickness);
            base.WriteInteger(data.FloorThickness);
            base.WriteInteger(0); // Mute settings
            base.WriteInteger(1); // Kick Settings
            base.WriteInteger(0); // Ban settings
            base.WriteInteger(1);
        }
    }
}
