using System;
using Mango.Communication.Sessions;
using Mango.Players;
using MySql.Data.MySqlClient;
using Mango.Communication.Packets.Outgoing.Handshake;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class SSOTicketEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            Session.TryAuthenticate(MySqlHelper.EscapeString(Packet.PopString().Trim()));
        }
    }
}
