using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Rooms.Mapping;
using Mango.Rooms;

namespace Mango.Communication.Packets.Incoming.Room.Avatar
{
    class LookToEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            RoomAvatar Avatar = Session.GetPlayer().GetAvatar();

            if (!Avatar.InRoom)
            {
                return;
            }

            if (Avatar.IsMoving)
            {
                return;
            }

            int PosX = Packet.PopWiredInt();
            int PosY = Packet.PopWiredInt();

            Vector2D Position = new Vector2D(PosX, PosY);

            if (Avatar.Position.X == Position.X && 
                Avatar.Position.Y == Position.Y || Avatar.HasStatus("lay") || Avatar.HasStatus("sit"))
            {
                return;
            }

            int NewRotation = AvatarRotation.Calculate(Avatar.Position.ToVector2D(), Position);

            if (Avatar.HeadRotation != NewRotation)
            {
                Avatar.HeadRotation = NewRotation;
                Avatar.UpdateNeeded = true;
            }

            if (Avatar.BodyRotation != NewRotation)
            {
                Avatar.BodyRotation = NewRotation;
                Avatar.UpdateNeeded = true;
            }

            Avatar.UnIdle();
        }
    }
}
