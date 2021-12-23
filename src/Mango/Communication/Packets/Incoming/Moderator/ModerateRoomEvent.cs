
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class ModerateRoomEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            if (!Session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            RoomInstance CurrentRoom = Session.GetPlayer().GetAvatar().GetCurrentRoom();
            int RoomId = Packet.PopWiredInt();

            if (CurrentRoom.Id != RoomId)
            {
                return;
            }

            bool SetLock = Packet.PopWiredBoolean();
            bool SetName = Packet.PopWiredBoolean();
            bool KickAll = Packet.PopWiredBoolean();

            CurrentRoom.UpdateDetails(SetName ? "Inappropriate to Hotel Management" : CurrentRoom.Name,
                SetName ? "Inappropriate to Hotel Management" : CurrentRoom.Description, SetLock ? RoomAccess.Locked : CurrentRoom.Access,
                SetLock ? string.Empty : CurrentRoom.Password, CurrentRoom.MaxUsers, CurrentRoom.CategoryId, CurrentRoom.Tags, CurrentRoom.AllowPets,
                CurrentRoom.AllowPetsEating, CurrentRoom.DisableRoomBlocking, CurrentRoom.HideWalls, CurrentRoom.WallThickness,
                CurrentRoom.FloorThickness);

            if (KickAll)
            {
                CurrentRoom.GetAvatars().RemoveAllGracefully(true);
            }
        }
    }
}
