using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Navigator;
using Mango.Rooms;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Global;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class OfficialRoomsComposer : ServerPacket
    {
        public OfficialRoomsComposer(ICollection<NavigatorOfficial> Items)
            : base(ServerPacketHeadersNew.OfficialRoomsComposer)
        {
            base.WriteInteger(Items.Count);

            foreach (NavigatorOfficial Item in Items)
            {
                if (Item.ParentId != 0)
                {
                    continue;
                }

                WriteOfficial(Item);

                if (Item.Category)
                {
                    foreach (NavigatorOfficial child in Items)
                    {
                        if (child.ParentId != Item.Id)
                        {
                            continue;
                        }

                        WriteOfficial(child);
                    }
                }
            }
            base.WriteInteger(0);
            base.WriteInteger(0);
        }

        private void WriteOfficial(NavigatorOfficial Item)
        {
            RoomData Data = null;

            if (!Item.TryGetRoomData(out Data) && !Item.Category)
            {
                WriteMissingOfficial(Item);
                return; // don't write this official, room data is missing.  -- TO-DO: Replace this with "WriteMissingOfficial" to replace with 'dummy data'
            }

            Item.TryGetRoomData(out Data);

            int type = 2;

            if (Item.Category)
            {
                type = 4;
            }
            else if (Data.Type == RoomType.FLAT)
            {
                type = 2;
            }

            base.WriteInteger(Item.Id);
            base.WriteString(Item.Name);
            base.WriteString(Item.Description);
            base.WriteInteger(NavigatorOfficialDisplayTypeUtility.GetOfficialDisplayTypeNum(Item.DisplayType));
            base.WriteString(Item.BannerLabel);
            base.WriteString(Item.ImageType == NavigatorOfficialImageType.EXTERNAL ? Item.Image : string.Empty);
            base.WriteInteger(Item.ParentId);
            base.WriteInteger(!Item.Category ? Data.UsersNow : 0);
            base.WriteInteger(type);

            if (Item.Category)
            {
                base.WriteBoolean(Item.CategoryAutoExpand);
            }
            else if (Data.Type == RoomType.FLAT)
            {
                RoomAppender.WriteRoom(this, Data);
            }
        }

        // for filling in missing room data officials
        private void WriteMissingOfficial(NavigatorOfficial item)
        {
            base.WriteInteger(item.Id);
            base.WriteString("Unable to display, official room data is missing for room id: " + item.RoomId);
            base.WriteString("Unable to display, official room data is missing for room id: " + item.RoomId);
            base.WriteInteger(1);
            base.WriteString(item.BannerLabel);
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(4);
            base.WriteBoolean(false);
        }
    }
}
