using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;

namespace Mango.Communication.Packets.Incoming
{
    sealed class ClientPacket
    {
        private Encoding Encoding = Encoding.Default;

        public int Id { get; private set; }
        public byte[] Body;

        private int Position;

        public ClientPacket()
        {
        }

        public void Initialize(int Id, byte[] Body)
        {
            this.Id = Id;
            this.Body = Body;
            this.Position = 0;
        }

        private int Length
        {
            get
            {
                return this.Body.Length;
            }
        }

        public int RemainingLength
        {
            get
            {
                return Length - Position;
            }
        }

        public byte[] ReadBytes(int b)
        {
            if (b > this.RemainingLength)
            {
                b = this.RemainingLength;
            }

            byte[] data = new byte[b];

            for (int i = 0; i < b; i++)
            {
                data[i] = this.Body[Position++];
            }

            return data;
        }

        public byte[] PlainReadBytes(int b)
        {
            if (b > this.RemainingLength)
            {
                b = this.RemainingLength;
            }

            byte[] data = new byte[b];

            for (int x = 0, y = this.Position; x < b; x++, y++)
            {
                data[x] = this.Body[y];
            }

            return data;
        }

        public byte[] ReadFixedValue() // d
        {
            int len = HabboEncoding.DecodeInt16(ReadBytes(2));
            return ReadBytes(len);
        }

        public bool PopB64Boolean() // d
        {
            if (this.RemainingLength > 0 && Body[Position++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public int PopInt() // d
        {
            if (RemainingLength < 1)
            {
                return 0;
            }

            byte[] Data = PlainReadBytes(4);

            Int32 i = HabboEncoding.DecodeInt32(Data);

            Position += 4;

            return i;
        }

        public string PopString() // d
        {
            return PopString(this.Encoding);
        }

        public string PopString(Encoding enc) // d
        {
            return enc.GetString(ReadFixedValue());
        }

        public int PopFixedInt() // d
        {
            Int32 i = 0;

            string s = PopString(Encoding.ASCII);

            Int32.TryParse(s, out i);

            return i;
        }

        public bool PopWiredBoolean() // d
        {
            if (this.RemainingLength > 0 && Body[Position++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public int PopWiredInt() // d
        {
            if (RemainingLength < 1)
            {
                return 0;
            }

            byte[] Data = PlainReadBytes(4);

            Int32 i = HabboEncoding.DecodeInt32(Data);

            Position += 4;

            return i;
        }
    }
}
