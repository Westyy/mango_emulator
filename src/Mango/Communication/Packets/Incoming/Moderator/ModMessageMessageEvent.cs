using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Packets.Incoming.Moderator
{
    class ModMessageMessageEvent : IPacketEvent
    {
        public void parse(Sessions.Session Session, ClientPacket Packet)
        {
            if (!Session.GetPlayer().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int UserId = Packet.PopWiredInt();
            string Message = Packet.PopString();

            Player Player = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(UserId, out Player))
            {
                Session.SendPacket(new ModMessageComposer("This user is not online anymore."));
                return;
            }

            Player.GetSession().SendPacket(new ModMessageComposer("Message from Moderator:\n\n" + Message));
        }
    }
}
