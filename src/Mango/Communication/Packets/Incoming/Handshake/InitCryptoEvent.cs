using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Handshake;
using Mango.Communication.Encryption;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class InitCryptoEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            Session.SendPacket(new InitCryptoComposer(new BigInteger(DiffieHellman.GenerateRandomHexString(15), 16).ToString()));
        }
    }
}
