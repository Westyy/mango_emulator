using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;

namespace Mango.Communication
{
    sealed class SendDataToken
    {
        public Session Session
        {
            get;
            set;
        }

        public int SendBytesRemainingCount
        {
            get;
            set;
        }

        public int BytesSentAlreadyCount
        {
            get;
            set;
        }

        public byte[] DataToSend
        {
            get;
            set;
        }

        public SendDataToken(Session Session)
        {
            this.Session = Session;
            this.SendBytesRemainingCount = 0;
            this.BytesSentAlreadyCount = 0;
            this.DataToSend = null;
        }

        public void Reset()
        {
            this.SendBytesRemainingCount = 0;
            this.BytesSentAlreadyCount = 0;
            Array.Clear(DataToSend, 0, DataToSend.Length);
            this.DataToSend = null;
        }
    }
}
