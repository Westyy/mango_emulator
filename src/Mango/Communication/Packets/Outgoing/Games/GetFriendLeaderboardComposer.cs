using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Games
{
    class GetFriendLeaderboardComposer : ServerPacket
    {
        public GetFriendLeaderboardComposer(Player Player)
            : base(ServerPacketHeadersNew.GetFriendLeaderboardComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1);
            base.WriteInteger(Player.Id);
            base.WriteInteger(1);
            base.WriteInteger(1); // position?
            base.WriteString(Player.Username);
            base.WriteString(Player.Figure);
            base.WriteString(Player.Gender == PlayerGender.MALE ? "m" : "f");
            base.WriteInteger(1);
            base.WriteInteger(0);
        }
    }
}
