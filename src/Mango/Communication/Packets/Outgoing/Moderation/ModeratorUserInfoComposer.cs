using Mango.Players;
using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserInfoComposer : ServerPacket
    {
        public ModeratorUserInfoComposer(PlayerData Data) :
            base(ServerPacketHeadersNew.ModeratorUserInfoComposer)
        {
            base.WriteInteger(Data.Id);
            base.WriteString(Data.Username);
            base.WriteString(Data.Figure);
            base.WriteInteger((int)(UnixTimestamp.GetNow() - Data.TimestampRegistered) / 60);
            base.WriteInteger((int)(UnixTimestamp.GetNow() - Data.TimestampLastOnline) / 60);
            base.WriteBoolean(Data.Online);
            base.WriteInteger(Data.ModTickets);
            base.WriteInteger(Data.ModTicketsAbusive);
            base.WriteInteger(Data.ModCautions);
            base.WriteInteger(Data.ModBans);
            base.WriteString("127.0.0.1"); // todo: ipadress
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteString("lol@lol.com"); // todo: email
        }
    }
}
