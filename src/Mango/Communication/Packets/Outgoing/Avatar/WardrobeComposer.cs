using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Players.Wardrobe;

namespace Mango.Communication.Packets.Outgoing.Avatar
{
    class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(Dictionary<int, WardrobeItem> Wardrobe)
            : base(ServerPacketHeadersNew.WardrobeMessageComposer)
        {
            base.WriteInteger(1); // can use wardrobe
            base.WriteInteger(Wardrobe.Count);

            foreach (KeyValuePair<int, WardrobeItem> Item in Wardrobe)
            {
                base.WriteInteger(Item.Key);
                base.WriteString(Item.Value.Figure);
                base.WriteString(Item.Value.Gender == PlayerGender.MALE ? "M" : "F");
            }
        }
    }
}
