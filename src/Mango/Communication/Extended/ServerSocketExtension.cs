using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Mango.Communication.Extended
{
    static class ServerSocketExtension
    {
        public static bool IsSocketConnected(Socket Socket)
        {
            if (Socket == null) { return false; }
            return !((Socket.Poll(1000, SelectMode.SelectRead) && (Socket.Available == 0)) || !Socket.Connected);
        }
    }
}
