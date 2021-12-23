using Mango.Communication.Packets.Incoming;
using Mango.Communication.Sessions;

namespace Mango.Communication.Packets
{
    interface IPacketEvent
    {
        void parse(Session Session, ClientPacket Packet);
    }
}
