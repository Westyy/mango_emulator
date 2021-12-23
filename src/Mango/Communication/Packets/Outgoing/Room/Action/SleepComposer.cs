using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Action
{
    class SleepComposer : ServerPacket
    {
        public SleepComposer(RoomAvatar avatar, bool sleeping)
            : base(ServerPacketHeadersNew.SleepMessageComposer)
        {
            base.WriteInteger(avatar.Player.Id);
            base.WriteBoolean(sleeping);
        }
    }
}
