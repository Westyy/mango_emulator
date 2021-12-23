using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomInstance Instance) :
            base(ServerPacketHeadersNew.ModeratorRoomInfoComposer)
        {
            base.WriteInteger(Instance.Id);
            base.WriteInteger(Instance.UsersNow);
            base.WriteBoolean(Instance.OwnerInRoom); // owner in room
            base.WriteInteger(Instance.OwnerId);
            base.WriteString(Instance.OwnerName);
            base.WriteBoolean(Instance != null);
            base.WriteString(Instance.Name);
            base.WriteString(Instance.Description);
            base.WriteInteger(Instance.Tags.Count);

            foreach (string Tag in Instance.Tags)
            {
                base.WriteString(Tag);
            }

            base.WriteBoolean(false);
        }
    }
}
