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
    class RemoveBuddyEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int Amount = packet.PopWiredInt();

            if (Amount > 100)
            {
                Amount = 100;
            }

            if (Amount < 0)
            {
                return;
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    for (int i = 0; i < Amount; i++)
                    {
                        int UserId = packet.PopWiredInt();

                        if (!session.GetPlayer().GetMessenger().IsFriends(UserId))
                        {
                            continue;
                        }

                        DbCon.SetQuery("DELETE FROM `messenger_friendships` WHERE `user_1_id` = @u1 AND `user_2_id` = @u2;");
                        DbCon.AddParameter("u1", session.GetPlayer().Id);
                        DbCon.AddParameter("u2", UserId);
                        int AffectedRows = DbCon.ExecuteNonQuery();

                        if (AffectedRows < 1)
                        {
                            continue;
                        }

                        session.GetPlayer().GetMessenger().TryRemove(UserId);

                        DbCon.SetQuery("DELETE FROM `messenger_friendships` WHERE `user_1_id` = @u1 AND `user_2_id` = @u2;");
                        DbCon.AddParameter("u1", UserId);
                        DbCon.AddParameter("u2", session.GetPlayer().Id);
                        int AffectedRows2 = DbCon.ExecuteNonQuery();

                        if (AffectedRows2 < 1)
                        {
                            continue;
                        }

                        Player Player = null;

                        if (Mango.GetServer().GetPlayerManager().TryGet(UserId, out Player))
                        {
                            Player.GetMessenger().TryRemove(session.GetPlayer().Id);

                            Player.GetSession().SendPacket(new FriendListUpdateComposer(
                                    new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.Remove, session.GetPlayer()) }));
                        }

                        if (Player != null)
                        {
                            session.SendPacket(new FriendListUpdateComposer(
                                    new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.Remove, Player) }));
                        }
                        else
                        {
                            PlayerData Info = PlayerLoader.GetDataById(UserId);

                            if (Info != null)
                            {
                                session.SendPacket(new FriendListUpdateComposer(
                                    new List<MessengerUpdate>() { new MessengerUpdate(MessengerUpdateMode.Remove, Info) }));
                            }
                        }
                    }

                    DbCon.Commit();
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                }
            }
        }
    }
}
