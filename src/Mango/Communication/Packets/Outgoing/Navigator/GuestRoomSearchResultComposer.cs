using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Global;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class GuestRoomSearchResultComposer : ServerPacket
    {
        public GuestRoomSearchResultComposer(int categoryId, int mode, string userQuery, ICollection<RoomData> rooms, bool showEventData = false)
            : base(ServerPacketHeadersNew.GuestRoomSearchResultComposer)
        {
            //base.WriteInteger(categoryId);
            base.WriteInteger(mode);
            base.WriteString(userQuery);
            base.WriteInteger(rooms.Count);

            foreach (RoomData data in rooms)
            {
                RoomAppender.WriteRoom(this, data, data.Promotion);
            }
            base.WriteBoolean(false);
            // if true.. loads of shit
        }
    }
}
