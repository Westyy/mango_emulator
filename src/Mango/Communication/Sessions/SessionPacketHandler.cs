using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Packets.Incoming;
using log4net;

namespace Mango.Communication.Sessions
{
    sealed class SessionPacketHandler
    {
        private static ILog log = LogManager.GetLogger("Mango.Communication.Sessions.SessionPacketHandler");

        private readonly Dictionary<int, bool> _registered;

        private bool _authed;

        public SessionPacketHandler()
        {
            this._registered = new Dictionary<int, bool>();
            this._authed = false;
        }

        public void ExecutePacket(Session Session, ClientPacket Packet)
        {
            if (Packet.Id == ClientPacketHeader.SSOTicketMessageEvent && _authed)
            {
                return;
            }

            if (Packet.Id == ClientPacketHeader.SSOTicketMessageEvent)
            {
                this._authed = true;
            }

            Mango.GetServer().GetPacketManager().ExecutePacket(Session, Packet);
        }

        private void Register(int Id, bool State)
        {
            if (this._registered.ContainsKey(Id))
            {
                this._registered[Id] = State;
            }
            else
            {
                this._registered.Add(Id, State);
            }
        }

        public void Reset()
        {
            this._registered.Clear();
            this._authed = false;
        }
    }
}
