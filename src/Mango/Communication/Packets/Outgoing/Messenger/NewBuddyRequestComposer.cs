using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(PlayerData sender)
            : base(ServerPacketHeadersNew.NewBuddyRequestComposer)
        {
            base.WriteInteger(sender.Id);
            base.WriteString(sender.Username);
            base.WriteString(sender.Figure);
            //base.WriteString(sender.Id.ToString());
        }
    }
}
