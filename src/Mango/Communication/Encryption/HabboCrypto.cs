using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Communication.Encryption
{
    class HabboCrypto : DiffieHellman
    {
        private static BigInteger n = new BigInteger("86851DD364D5C5CECE3C883171CC6DDC5760779B992482BD1E20DD296888DF91B33B936A7B93F06D29E8870F703A216257DEC7C81DE0058FEA4CC5116F75E6EFC4E9113513E45357DC3FD43D4EFAB5963EF178B78BD61E81A14C603B24C8BCCE0A12230B320045498EDC29282FF0603BC7B7DAE8FC1B05B52B2F301A9DC783B7", 16);
        private static BigInteger e = new BigInteger(3);
        private static BigInteger d = new BigInteger("59AE13E243392E89DED305764BDD9E92E4EAFA67BB6DAC7E1415E8C645B0950BCCD26246FD0D4AF37145AF5FA026C0EC3A94853013EAAE5FF1888360F4F9449EE023762EC195DFF3F30CA0B08B8C947E3859877B5D7DCED5C8715C58B53740B84E11FBC71349A27C31745FCEFEEEA57CFF291099205E230E0C7C27E8E1C0512B", 16);

        private RSA RSA;

        public Boolean Initialized { get; private set; }

        public HabboCrypto()
            : base(200)
        {
            this.RSA = new RSA(n, e, d, 0, 0, 0, 0, 0);

            this.Initialized = false;
        }

        public HabboCrypto(BigInteger n, BigInteger e, BigInteger d)
            : base(new BigInteger("114670925920269957593299136150366957983142588366300079186349531", 10), new BigInteger("1589935137502239924254699078669119674538324391752663931735947", 10))
        {
            this.RSA = new RSA(n, e, d, 0, 0, 0, 0, 0);

            this.Initialized = false;
        }

        public Boolean InitializeRC4ToSession(Session Session, string ctext)
        {
            // return new BigInteger(clientKey, 10).modPow(this.iPrivateKey, this.m_banner.iPrime).toString(16).toUpperCase();
            try
            {
                string publickey = this.RSA.Decrypt(ctext);

                base.GenerateSharedKey(publickey.Replace(((char)0).ToString(), ""));

                Random HandlerRND = new Random();
                Session.DesignedHandler = HandlerRND.Next(1, 5);

                ///RC4.Init(base.SharedKey.getBytes(), ref Session.i, ref Session.j, ref Session.table);
                RC4.Init(base.SharedKey.getBytes(), ref Session.RC4Client);
                Session.CryptoInitialized = true;
                this.Initialized = true;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
