using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Encryption
{
    public class RC4
    {


        /**
         * Método globalizado para evitar Outs of Memory
         * By Itachy & Fakundo. << cappostrike
         * 2012
         */

        private int i = 0;
        private int j = 0;
        private int[] Table = new int[256];

        public RC4(byte[] key)
        {
            int k = key.Length;
            while (this.i < 256)
            {
                Table[this.i] = this.i;
                this.i++;
            }

            this.i = 0;
            this.j = 0;
            while (this.i < 256)
            {
                this.j = (((this.j + Table[this.i]) + key[(this.i % k)]) % 256);
                this.Swap(this.i, this.j);
                this.i++;
            }

            this.i = 0;
            this.j = 0;
        }

        public static void Init(byte[] key, ref RC4 client)
        {
            client = new RC4(key);
        }

        public void Swap(int a, int b)
        {
            int k = this.Table[a];
            this.Table[a] = this.Table[b];
            this.Table[b] = k;
        }

        public byte[] Parse(byte[] bytes)
        {
            int k = 0;
            byte[] result = new byte[bytes.Length];
            int pos = 0;

            for (int a = 0; a < bytes.Length; a++)
            {
                this.i = ((this.i + 1) % 256);
                this.j = ((this.j + this.Table[this.i]) % 256);
                this.Swap(this.i, this.j);
                k = ((this.Table[this.i] + this.Table[this.j]) % 256);
                result[pos++] = (byte)((bytes[a] & 0xff) ^ this.Table[k]);
            }

            return result;
        }
    }
}
