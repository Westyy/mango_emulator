using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players.Messenger;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class FriendListUpdateComposer : ServerPacket
    {
         public FriendListUpdateComposer(ICollection<MessengerUpdate> Updates)
            : base(ServerPacketHeadersNew.FriendListUpdateComposer)
        {
            base.WriteInteger(0);
            base.WriteInteger(Updates.Count);

            foreach (MessengerUpdate Update in Updates)
            {
                if (Update.Mode == MessengerUpdateMode.AddUpdate)
                {
                    base.WriteInteger(1);
                }
                else if (Update.Mode == MessengerUpdateMode.Update)
                {
                    base.WriteInteger(0);
                }
                else if (Update.Mode == MessengerUpdateMode.Remove)
                {
                    base.WriteInteger(-1);
                }
                else
                {
                    base.WriteInteger(Update.Player.Id);
                }

                base.WriteInteger(Update.Player.Id);

                if (Update.Mode != MessengerUpdateMode.Remove)
                {
                    base.WriteString(Update.Player.Username);
                    base.WriteInteger(1);
                    base.WriteBoolean(Update.Player.Online);
                    base.WriteBoolean(Update.Player.InRoom);
                    base.WriteString(Update.Player.Online ? Update.Player.Figure : string.Empty);
                    base.WriteInteger(0); // Category ID
                    base.WriteString(Update.Player.Online ? Update.Player.Motto : string.Empty);

                    /*if (Update.Player.Online)
                    {
                        base.WriteString(string.Empty);
                    }
                    else
                    {
                        if (Update.Player.TimestampLastOnline == 0)
                        {
                            base.WriteString("Never");
                        }
                        else
                        {
                            DateTime LastOnline = UnixTimestamp.FromUnixTimestamp(Update.Player.TimestampLastOnline);
                            base.WriteString(LastOnline.ToShortDateString() + " " + LastOnline.ToShortTimeString());
                        }
                    }*/

                    base.WriteString(Update.Player.AlternativeName);
                    base.WriteString(string.Empty);
                    base.WriteBoolean(false);
                    base.WriteBoolean(false);
                    base.WriteBoolean(false);
                    base.WriteShort(0);
                }
            }


        }
    }
}
