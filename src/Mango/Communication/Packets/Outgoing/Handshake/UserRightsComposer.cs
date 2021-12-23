using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class UserRightsComposer : ServerPacket
    {
        public UserRightsComposer(bool ClubRegular, bool ClubVIP, bool HotelAdmin)
            : base(ServerPacketHeadersNew.UserRightsMessageComposer)
        {
            if (!ClubRegular && ClubVIP)
            {
                base.WriteInteger(2);
            }
            else if (ClubRegular && !ClubVIP)
            {
                base.WriteInteger(1);
            }
            else
            {
                base.WriteInteger(0);
            }

            base.WriteInteger(HotelAdmin ? 1000 : 0);
        }
    }
}
