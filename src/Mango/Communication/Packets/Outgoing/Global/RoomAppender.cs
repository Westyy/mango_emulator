using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Global
{
    static class RoomAppender
    {
        public static void WriteRoom(ServerPacket Packet, RoomData data, RoomPromotion Promotion = null)
        {
            Packet.WriteInteger(data.Id);
            Packet.WriteString(data.Name);
            Packet.WriteBoolean(true);
            Packet.WriteInteger(data.OwnerId);
            Packet.WriteString(data.OwnerName);
            Packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(data.Access));
            Packet.WriteInteger(data.UsersNow);
            Packet.WriteInteger(data.MaxUsers);
            Packet.WriteString(data.Description);
            Packet.WriteInteger(0);
            Packet.WriteInteger(data.CanTrade ? 2 : 0);
            Packet.WriteInteger(data.Score);
            Packet.WriteInteger(0);
            Packet.WriteInteger(data.CategoryId);
            Packet.WriteInteger(0);
            Packet.WriteString("");
            Packet.WriteString("");
            Packet.WriteString("");
            Packet.WriteInteger(data.Tags.Count);

            foreach (string tag in data.Tags)
            {
                Packet.WriteString(tag);
            }

            Packet.WriteInteger(0);
            Packet.WriteInteger(0);
            Packet.WriteBoolean(false);
            Packet.WriteBoolean(false);
            Packet.WriteInteger(Promotion != null ? 1 : 0);
            Packet.WriteString(Promotion != null ? Promotion.Name : "");
            Packet.WriteString(Promotion != null ? Promotion.Description : "");
            Packet.WriteInteger(Promotion != null ? Promotion.MinutesLeft : 0);
        }
    }
}
