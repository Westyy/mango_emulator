using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Settings
{
    class RoomSettingsSavedComposer : ServerPacket
    {
        public RoomSettingsSavedComposer(RoomData data)
            : base(ServerPacketHeadersNew.RoomSettingsSavedComposer)
        {
            base.WriteInteger(data.Id);
        }
    }
}
