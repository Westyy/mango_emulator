using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer(ICollection<PlayerData> friends)
            : base(ServerPacketHeadersNew.MessengerInitComposer)
        {
            base.WriteInteger(300);
            base.WriteInteger(300);
            base.WriteInteger(800);
            base.WriteInteger(1100);
            base.WriteInteger(0); // category count
            base.WriteInteger(friends.Count);

            foreach (PlayerData player in friends)
            {
                base.WriteInteger(player.Id);
                base.WriteString(player.Username);
                base.WriteInteger(1);
                base.WriteBoolean(player.Online);
                base.WriteBoolean(player.Online && player.InRoom);
                base.WriteString(player.Online == true ? player.Figure : string.Empty);
                base.WriteInteger(0); // category id
                base.WriteString(player.Online == true ? player.Motto : string.Empty);

                base.WriteString(player.AlternativeName);
                /*if (player.Online)
                {
                    base.WriteString(string.Empty);
                }
                else
                {
                    DateTime LastOnline = UnixTimestamp.FromUnixTimestamp(player.TimestampLastOnline);
                    base.WriteString(LastOnline.ToShortDateString() + " " + LastOnline.ToShortTimeString());
                }*/

                //base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);
            }
        }
    }
}
