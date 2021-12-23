using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int HomeRoomId)
            : base(ServerPacketHeadersNew.NavigatorSettingsComposer)
        {
            base.WriteInteger(HomeRoomId);
            base.WriteInteger(HomeRoomId);
        }
    }
}
