using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players.Messenger;

namespace Mango.Communication.Packets.Outgoing.Messenger
{
    class InstantMessageErrorComposer : ServerPacket
    {
        public InstantMessageErrorComposer(MessengerMessageErrors error, int targetId)
            : base(ServerPacketHeadersNew.InstantMessageErrorComposer)
        {
            base.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(error));
            base.WriteInteger(targetId);
            base.WriteString("");
        }
    }
}
