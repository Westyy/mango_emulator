using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class HabboSearchResultComposer : ServerPacket
    {
        public HabboSearchResultComposer(ICollection<PlayerData> friends, ICollection<PlayerData> nonfriends)
            : base(ServerPacketHeadersNew.HabboSearchResultComposer)
        {
            base.WriteInteger(friends.Count);

            foreach (PlayerData player in friends)
            {
                base.WriteInteger(player.Id);
                base.WriteString(player.Username);
                base.WriteString(player.Motto);
                base.WriteBoolean(player.Online);
                base.WriteBoolean(player.InRoom);
                base.WriteString("");
                base.WriteInteger(0);
                base.WriteString(player.Online == true ? player.Figure : string.Empty);
                if (player.TimestampLastOnline == 0)
                {
                    base.WriteString("Never");
                }
                else
                {
                    DateTime LastOnline = UnixTimestamp.FromUnixTimestamp(player.TimestampLastOnline);
                    base.WriteString(LastOnline.ToShortDateString() + " " + LastOnline.ToShortTimeString());
                }
            }

            base.WriteInteger(nonfriends.Count);

            foreach (PlayerData player in nonfriends)
            {
                base.WriteInteger(player.Id);
                base.WriteString(player.Username);
                base.WriteString(player.Motto);
                base.WriteBoolean(player.Online);
                base.WriteBoolean(false); // In Room automatically set to false
                base.WriteString(string.Empty);
                base.WriteInteger(0);
                base.WriteString(player.Online == true ? player.Figure : string.Empty);
                if (player.TimestampLastOnline == 0)
                {
                    base.WriteString("Never");
                }
                else
                {
                    DateTime LastOnline = UnixTimestamp.FromUnixTimestamp(player.TimestampLastOnline);
                    base.WriteString(LastOnline.ToShortDateString() + " " + LastOnline.ToShortTimeString());
                }
            }
        }
    }
}
