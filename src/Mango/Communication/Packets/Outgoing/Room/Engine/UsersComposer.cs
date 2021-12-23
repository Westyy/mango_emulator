using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomAvatar> Avatars)
            : base(ServerPacketHeadersNew.UsersMessageComposer)
        {
            base.WriteInteger(Avatars.Count);

            foreach (RoomAvatar Avatar in Avatars)
            {
                WriteAvatar(Avatar);
            }
        }

        public UsersComposer(RoomAvatar Avatar)
            : base(ServerPacketHeadersNew.UsersMessageComposer)
        {
            base.WriteInteger(1); // 1 single avatar
            WriteAvatar(Avatar);
        }

        private void WriteAvatar(RoomAvatar Avatar)
        {
            bool IsBot = (Avatar.Type == RoomAvatarType.Bot);
            bool IsPet = (Avatar.Type == RoomAvatarType.Pet);

            if (IsBot)
            {
                base.WriteInteger(-1);
            }
            else
            {
                base.WriteInteger(Avatar.Player.Id);
            }

            base.WriteString(Avatar.Player.Username);
            base.WriteString(Avatar.Player.Motto);
            base.WriteString(Avatar.Player.Figure);
            base.WriteInteger(Avatar.Player.Id);
            base.WriteInteger(Avatar.Position.X);
            base.WriteInteger(Avatar.Position.Y);
            base.WriteDouble(Avatar.Position.Z);

            base.WriteInteger(Avatar.Type == RoomAvatarType.Player ? 2 : 4); // 2 for user 4 for bot
            base.WriteInteger(Avatar.Type == RoomAvatarType.Player ? 1 : Avatar.Type == RoomAvatarType.Pet ? 2 : 4); // 1 for user 2 for pet 3 for bot


            if (!IsBot)
            {
                base.WriteString(Avatar.Player.Gender == PlayerGender.MALE ? "m" : "f");
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteString("");
                base.WriteString("");
                base.WriteInteger(0);
            }
            else if (IsPet)
            {
                base.WriteInteger(500);
            }
        }
    }
}
