using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Handshake
{
    class UserPerksComposer : ServerPacket
    {
        public UserPerksComposer()
            : base(ServerPacketHeadersNew.UserPerksComposer)
        {
            // todo: permission shit for this?
            base.WriteInteger(9); // Count
            base.WriteString("SAFE_CHAT");
            base.WriteBoolean(true);
            base.WriteString(""); // requirement.unfulfilled.safety_quiz_1
            base.WriteString("USE_GUIDE_TOOL");
            base.WriteBoolean(false);
            base.WriteString("requirement.unfulfilled.helper_le");
            base.WriteString("GIVE_GUIDE_TOURS");
            base.WriteBoolean(false);
            base.WriteString(""); // ??
            base.WriteString("JUDGE_CHAT_REVIEWS");
            base.WriteBoolean(false);
            base.WriteString(""); // ??
            base.WriteString("CALL_ON_HELPERS");
            base.WriteBoolean(false);
            base.WriteString(""); // ??
            base.WriteString("CITIZEN");
            base.WriteBoolean(true);
            base.WriteString(""); // ??
            base.WriteString("FULL_CHAT");
            base.WriteBoolean(true);
            base.WriteString(""); // ??
            base.WriteString("TRADE");
            base.WriteBoolean(true);
            base.WriteString(""); // ??
            base.WriteString("VOTE_IN_COMPETITIONS");
            base.WriteBoolean(false);
            base.WriteString("requirement.unfulfilled.helper_level_2");
        }
    }
}
