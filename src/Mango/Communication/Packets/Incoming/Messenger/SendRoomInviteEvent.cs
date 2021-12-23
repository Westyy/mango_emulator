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
    class SendRoomInviteEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            int Amount = packet.PopWiredInt();
            List<int> Targets = new List<int>();

            if (Amount > 500)
            {
                return; // don't send at all
            }

            for (int i = 0; i < Amount; i++)
            {
                int uid = packet.PopWiredInt();

                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    Targets.Add(uid);
                }
            }

            string Message = StringCharFilter.Escape(packet.PopString());

            if (Message.Length > 121)
            {
                Message = Message.Substring(0, 121);
            }

            foreach (int UserId in Targets)
            {
                MessengerFriendship Friend = null;

                if (!session.GetPlayer().GetMessenger().TryGet(UserId, out Friend))
                {
                    continue;
                }

                Player Player = Friend.Friend;

                if (Player != null)
                {
                    Player.GetSession().SendPacket(new RoomInviteComposer(session.GetPlayer().Id, Message));
                }
            }
        }
    }
}
