using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using Mango.Communication.Sessions;
using Mango.Players;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Incoming.Messenger
{
    class DeclineBuddyEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            bool DeclineAll = packet.PopWiredBoolean();
            int Amount = packet.PopWiredInt();

            if (DeclineAll)
            {
                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    try
                    {
                        DbCon.Open();
                        DbCon.BeginTransaction();

                        DbCon.SetQuery("DELETE FROM `messenger_friendships` WHERE `user_1_id` = @uid AND `confirmed` = 0");
                        DbCon.AddParameter("uid", session.GetPlayer().Id);
                        DbCon.ExecuteNonQuery();

                        session.GetPlayer().GetMessenger().RemoveAllRequests();

                        DbCon.Commit();
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }
            }
            else
            {
                if (Amount < 1)
                {
                    return;
                }

                if (Amount > 100)
                {
                    Amount = 100;
                }

                using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    try
                    {
                        for (int i = 0; i < Amount; i++)
                        {
                            int RequestId = packet.PopWiredInt();

                            MessengerFriendship Request = null;

                            if (!session.GetPlayer().GetMessenger().TryRemoveRequest(RequestId, out Request))
                            {
                                continue;
                            }

                            DbCon.SetQuery("DELETE FROM `messenger_friendships` WHERE `id` = @id;");
                            DbCon.AddParameter("id", Request.Id);
                            DbCon.ExecuteNonQuery();

                            DbCon.Commit();
                        }
                    }
                    catch (MySqlException)
                    {
                        DbCon.Rollback();
                    }
                }
            }
        }
    }
}
