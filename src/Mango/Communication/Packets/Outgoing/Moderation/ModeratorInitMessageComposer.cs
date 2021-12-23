using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorInitMessageComposer : ServerPacket
    {
        public ModeratorInitMessageComposer(bool HasModTicketsRight, ICollection<string> UserMessagePresets, ICollection<string> RoomMessagePresets, Dictionary<string, Dictionary<string, string>> UserActionPresets) :
            base(ServerPacketHeadersNew.ModeratorInitMessageComposer)
        {
            base.WriteInteger(0);
            //SerializeTickets();
            base.WriteInteger(UserMessagePresets.Count);

            foreach (string Preset in UserMessagePresets)
            {
                base.WriteString(Preset);
            }

            base.WriteInteger(UserActionPresets.Count);

            foreach (KeyValuePair<string, Dictionary<string, string>> PresetCategory in UserActionPresets)
            {
                base.WriteString(PresetCategory.Key);
                base.WriteBoolean(true); // ?
                base.WriteInteger(PresetCategory.Value.Count);

                foreach (KeyValuePair<string, string> Preset in PresetCategory.Value)
                {
                    base.WriteString(Preset.Key);
                    base.WriteString(Preset.Value);
                    base.WriteInteger(1); // ?
                    base.WriteInteger(0); // Mute time for new style cfh
                    base.WriteInteger(0);
                    base.WriteString("");
                }
            }

            base.WriteBoolean(true);//HasModTicketsRight); // Tickets
            base.WriteBoolean(true); // Chatlogs
            base.WriteBoolean(true); // Message, user action, send caution
            base.WriteBoolean(true); // kick
            base.WriteBoolean(true); // ban
            base.WriteBoolean(true); // caution, message
            base.WriteBoolean(true); // ?

            base.WriteInteger(RoomMessagePresets.Count);

            foreach (string Preset in RoomMessagePresets)
            {
                base.WriteString(Preset);
            }
        }

        private void SerializeTickets()
        {
            base.WriteInteger(1); // Id
            base.WriteInteger(1); // Tab ID
            base.WriteInteger(3); // Type
            base.WriteInteger(114); // Category
            base.WriteInteger(0); // time in milliseconds?
            base.WriteInteger(1); // Priority
            base.WriteInteger(1); // Sender ID
            base.WriteString("Matty"); // Sender Name
            base.WriteInteger(2); // Reported ID
            base.WriteString("Tom"); // Reported Name
            base.WriteInteger(0); // Moderator ID
            base.WriteString(""); // Mod Name
            base.WriteString("He tells me to go naked on tinychat with him!!"); // Issue
            base.WriteInteger(5); // Room Id
            base.WriteString("Sex Dungeon"); // Room Name
            base.WriteInteger(0); // Is Public room
            base.WriteString("-1");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
