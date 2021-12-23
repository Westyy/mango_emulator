using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class FriendNotificationComposer : ServerPacket
    {
        public FriendNotificationComposer(PlayerData player, MessengerEventTypes type, string data)
            : base(ServerPacketHeadersNew.FriendNotificationComposer)
        {
            base.WriteString(player.Id.ToString());
            base.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(type));
            base.WriteString(data);
        }
    }
}
