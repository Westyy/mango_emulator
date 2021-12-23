using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class PopularRoomTagsResultComposer : ServerPacket
    {
        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> tags)
            : base(ServerPacketHeadersNew.PopularRoomTagsResultComposer)
        {
            base.WriteInteger(tags.Count);

            foreach (KeyValuePair<string, int> tag in tags)
            {
                base.WriteString(tag.Key);
                base.WriteInteger(tag.Value);
            }
        }
    }
}
