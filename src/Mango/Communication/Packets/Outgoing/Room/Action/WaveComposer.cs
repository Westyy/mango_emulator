using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Action
{
    class WaveComposer : ServerPacket
    {
        public WaveComposer(RoomAvatar avatar, int action)
            : base(ServerPacketHeadersNew.WaveMessageComposer)
        {
            base.WriteInteger(avatar.Player.Id);
            base.WriteInteger(action);
        }
    }
}
