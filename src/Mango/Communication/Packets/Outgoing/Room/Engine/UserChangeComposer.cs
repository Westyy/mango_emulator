using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(Player Player, bool Self)
            : base(ServerPacketHeadersNew.UserChangeMessageComposer)
        {
            if (Self)
            {
                base.WriteInteger(-1);
            }
            else
            {
                base.WriteInteger(Player.Id);
            }

            base.WriteString(Player.Figure);
            base.WriteString(Player.Gender == PlayerGender.MALE ? "m" : "f");
            base.WriteString(Player.Motto);
            base.WriteInteger(Player.Score);
        }
    }
}
