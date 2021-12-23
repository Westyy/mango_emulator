using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Room.Engine
{
    class UserUpdateComposer : ServerPacket
    {
        public UserUpdateComposer(ICollection<RoomAvatar> avatars)
            : base(ServerPacketHeadersNew.UserUpdateMessageComposer)
        {
            base.WriteInteger(avatars.Count);

            foreach (RoomAvatar avatar in avatars)
            {
                base.WriteInteger(avatar.Player.Id);
                base.WriteInteger(avatar.Position.X);
                base.WriteInteger(avatar.Position.Y);
                //base.WriteDouble(avatar.Position.Z);
                base.WriteString(avatar.Position.Z.ToString());
                base.WriteInteger(avatar.HeadRotation);
                base.WriteInteger(avatar.BodyRotation);

                StringBuilder StatusComposer = new StringBuilder();

                StatusComposer.Append("/");
                 
                foreach (KeyValuePair<string, string> Status in avatar.Statusses)
                {
                    StatusComposer.Append(Status.Key);

                    if (Status.Value != string.Empty)
                    {
                        StatusComposer.Append(" ");
                        StatusComposer.Append(Status.Value);
                    }

                    StatusComposer.Append("/");
                }

                StatusComposer.Append("/");
                base.WriteString(StatusComposer.ToString());
            }

            /*Message.AppendInt32(VirtualId);
            Message.AppendInt32(X);
            Message.AppendInt32(Y);
            Message.AppendString(TextHandling.GetString(Z));
            Message.AppendInt32(RotHead);
            Message.AppendInt32(RotBody);
            StringBuilder StatusComposer = new StringBuilder();
            StatusComposer.Append("/");

            foreach (KeyValuePair<string, string> Status in Statusses)
            {
                StatusComposer.Append(Status.Key);

                if (Status.Value != string.Empty)
                {
                    StatusComposer.Append(" ");
                    StatusComposer.Append(Status.Value);
                }

                StatusComposer.Append("/");
            }

            StatusComposer.Append("/");
            Message.AppendString(StatusComposer.ToString());*/
        }
    }
}
