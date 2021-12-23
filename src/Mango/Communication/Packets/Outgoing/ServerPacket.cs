using System;
using System.Collections.Generic;
using System.Text;
using Mango.Utilities;

namespace Mango.Communication.Packets.Outgoing
{
    class ServerPacket
    {
        private Encoding Encoding = Encoding.Default;

        public int Id { get; private set; }
        private List<byte> Body = new List<byte>();

        internal ServerPacket(int id)
        {
            this.Id = id;
            WriteShort(id);
        }

        internal void WriteBytes(byte[] b, bool IsInt) // d
        {
            if (IsInt)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                {
                    Body.Add(b[i]);
                }
            }
            else
            {
                Body.AddRange(b);
            }
        }

        internal void WriteRawString(string s)
        {
            this.Body.AddRange(Encoding.GetBytes(s));
        }

        internal void WriteDouble(double d) // d
        {
            string Raw = Math.Round(d, 1).ToString();

            if (Raw.Length == 1)
            {
                Raw += ".0";
            }

            WriteString(Raw.Replace(',', '.'));
        }

        internal void WriteString(string s) // d
        {
            WriteShort(s.Length);
            WriteBytes(Encoding.GetBytes(s), false);
        }

        public void WriteShort(int s) // d
        {
            Int16 i = (Int16)s;
            WriteBytes(BitConverter.GetBytes(i), true);
        }

        internal void WriteInteger(int i) // d
        {
            WriteBytes(BitConverter.GetBytes(i), true);
        }

        internal void WriteRawInteger(int i) // d
        {
            WriteString(i.ToString());
        }

        internal void WriteBoolean(bool b) // d
        {
            WriteBytes(new byte[] { (byte)(b ? 1 : 0) }, false);
        }

        public byte[] GetBytes()
        {
            List<byte> Final = new List<byte>();
            Final.AddRange(BitConverter.GetBytes(Body.Count)); // packet len
            Final.Reverse();
            Final.AddRange(Body); // Add Packet
            return Final.ToArray();
        }
    }
}
