using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms;

namespace Mango.Communication.Packets.Outgoing.Navigator
{
    class GetGuestRoomResultComposer : ServerPacket
    {
        public GetGuestRoomResultComposer(RoomData Data, bool IsLoading = true, bool CheckEntry = false)
            : base(ServerPacketHeadersNew.GetGuestRoomResultComposer)
        {
            base.WriteBoolean(IsLoading); // I thought this was has group
            //base.WriteBoolean(false);
            base.WriteInteger(Data.Id);
            base.WriteString(Data.Name);
            base.WriteBoolean(true);
            base.WriteInteger(Data.OwnerId);
            base.WriteString(Data.OwnerName);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            base.WriteInteger(Data.UsersNow);
            base.WriteInteger(Data.MaxUsers);
            base.WriteString(Data.Description);
            base.WriteInteger(0);
            base.WriteInteger(Data.CanTrade ? 2 : 0);
            base.WriteInteger(Data.Score);
            base.WriteInteger(0);
            base.WriteInteger(Data.CategoryId);
            base.WriteInteger(0); // Group Id
            base.WriteString(""); // Group name
            base.WriteString(""); // Group badge
            base.WriteString("");
            base.WriteInteger(Data.Tags.Count);

            foreach (string tag in Data.Tags)
            {
                base.WriteString(tag);
            }

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteBoolean(false);
            base.WriteBoolean(false);
            base.WriteString("");
            base.WriteString("");
            base.WriteInteger(0);

            base.WriteBoolean(CheckEntry);
            base.WriteBoolean(false); 
            base.WriteBoolean(false);
            base.WriteBoolean(false);
            base.WriteInteger(0); // mute settings
            base.WriteInteger(1); // kick settings
            base.WriteInteger(0); // ban settings
            base.WriteBoolean(true);
            /*
            base.WriteBoolean(IsLoading);
            base.WriteInteger(Data.Id);
            base.WriteInteger(0);
            base.WriteString(Data.Name);
            base.WriteString(Data.OwnerName);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            base.WriteInteger(Data.UsersNow);
            base.WriteInteger(Data.MaxUsers);
            base.WriteString(Data.Description);
            base.WriteInteger(0);
            base.WriteBoolean(Data.CanTrade);
            base.WriteInteger(0);
            base.WriteInteger(Data.CategoryId);
            base.WriteString(string.Empty);
            base.WriteInteger(Data.Tags.Count);

            foreach (string tag in Data.Tags)
            {
                base.WriteString(tag);
            }

            base.WriteInteger(Data.Icon.BackgroundImageId);
            base.WriteInteger(Data.Icon.OverlayImageId);
            base.WriteInteger(Data.Icon.Objects.Count);

            foreach (KeyValuePair<int, int> obj in Data.Icon.Objects)
            {
                base.WriteInteger(obj.Key);
                base.WriteInteger(obj.Value);
            }

            base.WriteBoolean(Data.AllowPets);
            base.WriteBoolean(true);
            base.WriteBoolean(CheckEntry);
            base.WriteBoolean(false);
             * */
        }
    }
}
