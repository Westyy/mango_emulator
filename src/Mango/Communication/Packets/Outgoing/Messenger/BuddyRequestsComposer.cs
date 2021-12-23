using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<PlayerData> players)
            : base(ServerPacketHeadersNew.BuddyRequestsComposer)
        {
            base.WriteInteger(players.Count);
            base.WriteInteger(players.Count);

            foreach (PlayerData player in players)
            {
                base.WriteInteger(player.Id);
                base.WriteString(player.Username);
                base.WriteString(player.Figure);
                //base.WriteInteger(0);not sure about ths
                //base.WriteString(player.Id.ToString());
            }
        }
    }
}
