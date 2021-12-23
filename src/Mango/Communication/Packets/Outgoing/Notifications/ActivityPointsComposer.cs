using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int Amount)
            : base(ServerPacketHeadersNew.ActivityPointsMessageComposer)
        {
            base.WriteInteger(1); // type

            base.WriteInteger(0); // other currency lovehearts etc
            base.WriteInteger(Amount);
        }
    }
}
