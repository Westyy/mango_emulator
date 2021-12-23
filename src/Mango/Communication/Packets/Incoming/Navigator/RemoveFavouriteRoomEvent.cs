using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Players.Favourites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopWiredInt();

            FavouriteRoom Favourite = null;

            if (Session.GetPlayer().Favourites().TryRemove(RoomId, out Favourite))
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    DbCon.SetQuery("DELETE FROM `user_favourites` WHERE `id` = @id;");
                    DbCon.AddParameter("id", Favourite.Id);
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
            }

            Session.SendPacket(new UpdateFavouriteRoomComposer(RoomId, false));
        }
    }
}
