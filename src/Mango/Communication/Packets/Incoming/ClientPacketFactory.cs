using Mango.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming
{
    static class ClientPacketFactory
    {
        private static ObjectPool<ClientPacket> _packetPool = new ObjectPool<ClientPacket>(() => new ClientPacket());

        public static ClientPacket CreateNew()
        {
            return new ClientPacket();
        }
    }
}
