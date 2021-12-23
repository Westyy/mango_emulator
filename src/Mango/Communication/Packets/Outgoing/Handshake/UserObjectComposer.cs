using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(PlayerData Player)
            : base(ServerPacketHeadersNew.UserObjectComposer)
        {
            base.WriteInteger(Player.Id);
            base.WriteString(Player.Username);
            base.WriteString(Player.Figure);
            base.WriteString(Player.Gender == PlayerGender.Male ? "M" : "F");
            base.WriteString(Player.Motto);
            base.WriteString("");
            base.WriteBoolean(false);
            base.WriteInteger(Player.PlayerStats.RespectPoints);
            base.WriteInteger(Player.PlayerStats.RespectPointsLeftPlayer);
            base.WriteInteger(Player.PlayerStats.RespectPointsLeftPet);
            base.WriteBoolean(true); // Friends stream active
            base.WriteString(""); // last online?
            base.WriteBoolean(false); // Can change name
            base.WriteBoolean(false);
        }
    }
}
