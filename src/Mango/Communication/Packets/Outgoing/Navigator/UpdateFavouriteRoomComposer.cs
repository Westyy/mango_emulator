using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer(int RoomId, bool Added)
            : base(ServerPacketHeadersNew.FavouriteChangedComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteBoolean(Added);
        }
    }
}
