using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(ICollection<int> FavouriteRoomIds)
            : base(ServerPacketHeadersNew.FavouritesComposer)
        {
            base.WriteInteger(MangoStaticSettings.MaximumFavouriteRooms);
            base.WriteInteger(FavouriteRoomIds.Count);

            foreach (int id in FavouriteRoomIds)
            {
                base.WriteInteger(id);
            }
        }
    }
}
