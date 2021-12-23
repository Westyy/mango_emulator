using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Messenger;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class RequestBuddyEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            string RequestUsername = StringCharFilter.Escape(Packet.PopString());
            PlayerData RequestInfo = PlayerLoader.GetInfoByUsername(RequestUsername);

            if (RequestInfo == null)
            {
                return; // to-do: handle missing players
            }

            if (RequestInfo.Id == Session.GetPlayer().Id)
            {
                return;
            }

            if (!RequestInfo.AllowFriendRequests)
            {
                Session.SendPacket(new MessengerErrorComposer(39, 3));
                return;
            }

            if (Session.GetPlayer().GetMessenger().IsFriends(RequestInfo.Id))
            {
                return;
            }

            Player Player = null;

            if (Mango.GetServer().GetPlayerManager().TryGet(RequestInfo.Id, out Player))
            {
                MessengerFriendship Request = null;

                if (Player.GetMessenger().TryGetRequest(RequestInfo.Id, out Request))
                {
                    return; // request already exists
                }
            }

            if (Player == null) // if its null then the checks did not run, lets request the database as the player is offline
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.SetQuery("SELECT EXISTS(SELECT * FROM `messenger_friendships` WHERE `user_1_id` = @u1 AND `user_2_id` = @u2)");
                        DbCon.AddParameter("u1", RequestInfo.Id);
                        DbCon.AddParameter("u2", Session.GetPlayer().Id);

                        int Result = DbCon.ExecuteSingleInt();

                        if (Result > 0)
                        {
                            return;
                        }
                    }
                    catch (MySqlException) { }
                }
            }

            MessengerFriendship Friendship = null;

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("INSERT INTO `messenger_friendships` (user_1_id,user_2_id,confirmed) VALUES(@u1,@u2,0);");
                    DbCon.AddParameter("u1", RequestInfo.Id);
                    DbCon.AddParameter("u2", Session.GetPlayer().Id);
                    DbCon.ExecuteNonQuery();

                    Friendship = new MessengerFriendship(DbCon.SelectLastId(), RequestInfo.Id, Session.GetPlayer().Id, false);

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return;
                }
            }

            if (Player != null)
            {
                if (Player.GetMessenger().TryAddRequest(Friendship))
                {
                    Player.GetSession().SendPacket(new NewBuddyRequestComposer(Session.GetPlayer()));
                }
            }
        }
    }
}
