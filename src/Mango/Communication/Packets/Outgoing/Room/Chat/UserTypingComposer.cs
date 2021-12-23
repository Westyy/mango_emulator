using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Room.Chat
{
    class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer(PlayerData player, bool IsTyping)
            : base(ServerPacketHeadersNew.UserTypingMessageComposer)
        {
            base.WriteInteger(player.Id);
            base.WriteInteger(IsTyping ? 1 : 0);
        }
    }
}
