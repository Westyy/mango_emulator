using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Players.Messenger;
using Mango.Communication.Packets.Outgoing.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class AcceptBuddyEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            int Amount = Packet.PopWiredInt();

            if (Amount > 100)
            {
                Amount = 100;
            }

            if (Amount < 0)
            {
                return;
            }

            for (int i = 0; i < Amount; i++)
            {
                int RequestUserId = Packet.PopWiredInt();

                if (Session.GetPlayer().GetMessenger().IsFriends(RequestUserId))
                {
                    continue;
                }

                MessengerFriendship Friendship = null;

                if (Session.GetPlayer().GetMessenger().TryRemoveRequest(RequestUserId, out Friendship))
                {
                    if (Friendship == null)
                    {
                        using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                        {
                            try
                            {
                                DbCon.Open();
                                DbCon.SetQuery("SELECT EXISTS(SELECT * FROM `messenger_friendships` WHERE `user_1_id` = @u1 AND `user_2_id` = @u2 AND `confirmed` = 0)");
                                DbCon.AddParameter("u1", Session.GetPlayer().Id);
                                DbCon.AddParameter("u2", RequestUserId);

                                int Result = DbCon.ExecuteSingleInt();

                                if (Result != 1)
                                {
                                    continue;
                                }
                            }
                            catch (MySqlException) { }
                        }
                    }

                    if (Session.GetPlayer().GetMessenger().TryAdd(Friendship))
                    {
                        Friendship.Confirmed = true;

                        MessengerFriendship FriendshipTwo = null;

                        using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                        {
                            try
                            {
                                DbCon.Open();
                                DbCon.BeginTransaction();

                                DbCon.SetQuery("UPDATE `messenger_friendships` SET `confirmed` = 1 WHERE `id` = @id;");
                                DbCon.AddParameter("id", Friendship.Id);
                                DbCon.ExecuteNonQuery();

                                DbCon.SetQuery("REPLACE INTO `messenger_friendships` (user_1_id,user_2_id,confirmed) VALUES(@u1,@u2,@con);");
                                DbCon.AddParameter("u1", RequestUserId);
                                DbCon.AddParameter("u2", Session.GetPlayer().Id);
                                DbCon.AddParameter("con", 1);
                                DbCon.ExecuteNonQuery();

                                FriendshipTwo = new MessengerFriendship(DbCon.SelectLastId(), RequestUserId, Session.GetPlayer().Id, true);

                                DbCon.Commit();
                            }
                            catch (MySqlException)
                            {
                                DbCon.Rollback();
                                return;
                            }
                        }

                        Player TargetPlayer = null;

                        if (Mango.GetServer().GetPlayerManager().TryGet(RequestUserId, out TargetPlayer))
                        {
                            TargetPlayer.GetMessenger().TryAdd(FriendshipTwo);

                            TargetPlayer.GetSession().SendPacket(new FriendListUpdateComposer(
                                new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.Update, Session.GetPlayer()) }));

                            TargetPlayer.GetSession().SendPacket(new FriendListUpdateComposer(
                                new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.AddUpdate, Session.GetPlayer()) }));

                            Session.SendPacket(new FriendListUpdateComposer(
                                new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.Update, TargetPlayer) }));

                            Session.SendPacket(new FriendListUpdateComposer(
                                new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.AddUpdate, TargetPlayer) }));
                        }
                    }
                    else
                    {
                        // cleanup
                    }
                }
            }
        }
    }
}
