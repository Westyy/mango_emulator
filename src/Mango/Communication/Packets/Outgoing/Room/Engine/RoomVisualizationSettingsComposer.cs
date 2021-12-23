using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class RoomVisualizationSettingsComposer : ServerPacket
    {
        public RoomVisualizationSettingsComposer(bool WallsHidden, int WallThickness, int FloorThickness)
            : base(ServerPacketHeadersNew.RoomVisualizationSettingsComposer)
        {
            base.WriteBoolean(WallsHidden);
            base.WriteInteger(WallThickness);
            base.WriteInteger(FloorThickness);
        }
    }
}
