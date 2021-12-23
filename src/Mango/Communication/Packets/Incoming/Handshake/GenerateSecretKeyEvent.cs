using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Communication.Sessions;
using Mango.Communication.Packets.Outgoing.Handshake;
using log4net;

namespace Mango.Communication.Packets.Incoming.Handshake
{
    class GenerateSecretKeyEvent : IPacketEvent
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Communication.Packets.Incoming.Handshake.GenerateSecretKeyEvent");

        public void parse(Session Session, ClientPacket Packet)
        {
            string CipherPublicKey = Packet.PopString();

            if (!Mango.GetServer().GetCrypto().InitializeRC4ToSession(Session, CipherPublicKey))
            {
                log.Error("Error in secret key.");
                Session.Disconnect();
                return;
            }

            Session.SendPacket(new SecretKeyComposer(Mango.GetServer().GetCrypto().PublicKey.ToString()));
        }
    }
}
