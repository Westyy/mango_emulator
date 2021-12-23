using Mango.Players;
using Mango.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Outgoing.Users
{
    class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Player Me, PlayerData Data, Player OnlinePlayer = null)
            : base(ServerPacketHeadersNew.ProfileInformationComposer)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Data.TimestampRegistered);

            base.WriteInteger(Data.Id);
            base.WriteString(Data.Username);
            base.WriteString(Data.Figure);
            base.WriteString(Data.Motto);
            base.WriteString(Origin.ToString("dd/MM/yyyy"));
            base.WriteInteger(Data.Score);
            base.WriteInteger(0); // friend count
            base.WriteBoolean(Me.Id != Data.Id && OnlinePlayer != null && OnlinePlayer.GetMessenger().IsFriends(Me.Id));
            base.WriteBoolean(Me.Id != Data.Id && OnlinePlayer != null && OnlinePlayer.GetMessenger().HasRequest(Me.Id));
            base.WriteBoolean(OnlinePlayer != null && OnlinePlayer.Online);

            base.WriteInteger(0); // Groups

            /*foreach (GroupUser G in Groups)
            {
                Group Group = PlusEnvironment.GetGame().GetGroupManager().GetGroup(G.GroupId);
                if (Group != null)
                {
                    Response.AppendUInt(Group.Id); //GroupId
                    Response.AppendString(Group.Name); // Group Name 
                    Response.AppendString(Group.Badge); // Group badge
                    Response.AppendString((PlusEnvironment.GetGame().GetGroupManager().SymbolColours.ContainsKey(Group.Colour1)) ? PlusEnvironment.GetGame().GetGroupManager().SymbolColours[Group.Colour1].Colour : ""); // Group Colour 1
                    Response.AppendString((PlusEnvironment.GetGame().GetGroupManager().BackGroundColours.ContainsKey(Group.Colour2)) ? PlusEnvironment.GetGame().GetGroupManager().BackGroundColours[Group.Colour2].Colour : ""); // Group Colour 2
                    Response.AppendBoolean(Group.Id == Data.FavouriteGroup); // Favourite Group
                }
                else
                {
                    Response.AppendInt32(1);
                    Response.AppendString("Invalid Group");
                    Response.AppendString("");
                    Response.AppendString("");
                    Response.AppendString("");
                    Response.AppendBoolean(false);
                }
            }*/

            base.WriteInteger((int)(Math.Round(UnixTimestamp.GetNow()) - Math.Round(Data.TimestampLastOnline)));
            base.WriteBoolean(true);
        }
    }
}
