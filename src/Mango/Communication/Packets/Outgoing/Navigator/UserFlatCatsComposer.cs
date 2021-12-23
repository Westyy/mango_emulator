using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Navigator;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class UserFlatCatsComposer : ServerPacket
    {
        public UserFlatCatsComposer(ICollection<FlatCategory> categories)
            : base(ServerPacketHeadersNew.UserFlatCatsComposer)
        {
            base.WriteInteger(categories.Count);

            foreach (FlatCategory category in categories)
            {
                base.WriteInteger(category.Id);
                base.WriteString(category.Title);
                base.WriteBoolean(category.Visible);
            }
        }
    }
}
