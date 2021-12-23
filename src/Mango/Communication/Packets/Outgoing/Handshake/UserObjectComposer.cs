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
            base.WriteString(Player.Gender == PlayerGender.MALE ? "M" : "F");
            base.WriteString(Player.Motto);
            base.WriteString(Player.AlternativeName);
            base.WriteBoolean(false);
            base.WriteInteger(Player.RespectPoints);
            base.WriteInteger(Player.RespectPointsLeftPlayer);
            base.WriteInteger(Player.RespectPointsLeftPet);
            base.WriteBoolean(true); // Friends stream active
            base.WriteString(""); // last online?
            base.WriteBoolean(false); // Can change name
            base.WriteBoolean(false);
        }
    }
}
