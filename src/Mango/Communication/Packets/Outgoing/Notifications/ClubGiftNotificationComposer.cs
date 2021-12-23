using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Notifications
{
    class ClubGiftNotificationComposer : ServerPacket
    {
        public ClubGiftNotificationComposer(int amountOfGifts)
            : base(ServerPacketHeader.ClubGiftNotificationComposer)
        {
            base.WriteInteger(amountOfGifts);
        }
    }
}
