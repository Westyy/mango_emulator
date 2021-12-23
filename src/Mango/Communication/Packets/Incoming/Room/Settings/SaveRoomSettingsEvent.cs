using System.Collections.Generic;
using Mango.Communication.Sessions;
using Mango.Rooms;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Room.Settings;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Navigator;

namespace Mango.Communication.Packets.Incoming.Room.Settings
{
    class SaveRoomSettingsEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            RoomInstance instance = session.GetPlayer().GetAvatar().GetCurrentRoom();

            if (!session.GetPlayer().GetAvatar().InRoom)
            {
                return;
            }

            if (!instance.GetRights().CheckRights(session.GetPlayer().GetAvatar(), true))
            {
                return;
            }

            int expectedRoomId = packet.PopWiredInt();

            if (instance.Id != expectedRoomId)
            {
                return;
            }

            string name = StringCharFilter.Escape(packet.PopString()).Trim();
            string description = StringCharFilter.Escape(packet.PopString()).Trim();
            RoomAccess access = RoomAccessUtility.ToRoomAccess(packet.PopWiredInt());
            string password = StringCharFilter.Escape(packet.PopString()).Trim();
            int userLimit = packet.PopWiredInt();
            int categoryId = packet.PopWiredInt();
            int tagCount = packet.PopWiredInt();

            List<string> tags = new List<string>();

            for (int i = 0; (i < tagCount && i < 2); i++)
            {
                string Tag = StringCharFilter.Escape(packet.PopString()).Trim().ToLower();

                if (Tag.Length > 32)
                {
                    Tag = Tag.Substring(0, 32);
                }

                if (Tag.Length > 0 && !tags.Contains(Tag))
                {
                    tags.Add(Tag);
                }
            }
            int undef = packet.PopWiredInt();
            bool allowPets = packet.PopWiredBoolean(); // (packet.ReadBytes(1)[0] == 65);
            bool allowPetEating = packet.PopWiredBoolean(); //(packet.ReadBytes(1)[0] == 65);
            bool allowBlocking = packet.PopWiredBoolean(); // (packet.ReadBytes(1)[0] == 65);
            bool hideWalls = packet.PopWiredBoolean(); //(packet.ReadBytes(1)[0] == 65);
            int wallThickness = packet.PopWiredInt();
            int floorThickness = packet.PopWiredInt();
            int whocanmute = packet.PopWiredInt();
            int whocankick = packet.PopWiredInt();
            int whocanban = packet.PopWiredInt();

            if (wallThickness < -2 || wallThickness > 1) // to-do: rights check here?
            {
                wallThickness = 0;
            }

            if (floorThickness < -2 || floorThickness > 1) // to-do: rights check here?
            {
                floorThickness = 0;
            }

            if (hideWalls && !session.GetPlayer().GetPermissions().HasRight("club_vip"))
            {
                hideWalls = false;
            }

            if (name.Length > 60)
            {
                name = name.Substring(0, 60);
            }

            if (description.Length > 128)
            {
                description = description.Substring(0, 128);
            }

            if (password.Length > 64)
            {
                password = password.Substring(0, 64);
            }

            if (userLimit > instance.Model.MaxUsers)
            {
                userLimit = instance.Model.MaxUsers;
            }

            if (userLimit < 1)
            {
                userLimit = 10;
            }

            if (name.Length == 0)
            {
                name = "Room";
            }

            if (access == RoomAccess.Password_Protected && password.Length == 0)
            {
                access = RoomAccess.Open;
            }

            if (instance.UpdateDetails(name, description, access, password, userLimit, categoryId, tags, allowPets,
                allowPetEating, allowBlocking, hideWalls, wallThickness, floorThickness))
            {
                session.SendPacket(new RoomSettingsSavedComposer(instance));
                session.SendPacket(new RoomInfoUpdatedComposer(instance));
                session.SendPacket(new RoomVisualizationSettingsComposer(hideWalls, wallThickness, floorThickness));
            }
        }
    }
}
