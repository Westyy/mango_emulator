using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Players.Favourites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class AddFavouriteRoomEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopWiredInt();

            FavouriteRoom Fav = new FavouriteRoom(0, RoomId, Session.GetPlayer().Id);

            if (Session.GetPlayer().Favourites().TryAdd(Fav))
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    DbCon.SetQuery("INSERT INTO `user_favourites` (user_id,room_id) VALUES(@uid,@roomid);");
                    DbCon.AddParameter("uid", Session.GetPlayer().Id);
                    DbCon.AddParameter("roomid", RoomId);
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.ExecuteNonQuery();
                    DbCon.Commit();
                }
            }

            Session.SendPacket(new UpdateFavouriteRoomComposer(RoomId, true));
        }
    }
}
