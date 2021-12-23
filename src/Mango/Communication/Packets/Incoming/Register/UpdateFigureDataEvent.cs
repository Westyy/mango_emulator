using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Players.Wardrobe;

namespace Mango.Communication.Packets.Incoming.Register
{
    class UpdateFigureDataEvent : IPacketEvent
    {
        public void parse(Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetWardrobe().CanChangeFigure)
            {
                return;
            }

            string NewGender = Packet.PopString().ToLower();
            string NewFigure = StringCharFilter.Escape(Packet.PopString());

            if (NewGender != "m" && NewGender != "f")
            {
                NewGender = "m";
            }

            if (NewFigure.Length == 0 || (NewFigure == Session.GetPlayer().Figure))
            {
                return;
            }

            if (!FigureValidation.Validate(NewFigure))
            {
                Session.SendPacket(new ModMessageComposer("Error."));
                return;
            }

            Session.GetPlayer().UpdateFigure(NewFigure);
            Session.GetPlayer().UpdateGender(NewGender);
            Session.SendPacket(new UserChangeComposer(Session.GetPlayer(), true));

            if (Session.GetPlayer().GetAvatar().InRoom)
            {
                Session.GetPlayer().GetAvatar().GetCurrentRoom().GetAvatars().BroadcastPacket(new UserChangeComposer(Session.GetPlayer(), false));
            }

            Session.GetPlayer().GetWardrobe().SetFigureUpdated();
            Session.GetPlayer().GetMessenger().SetUpdateNeeded(false);
        }
    }
}
