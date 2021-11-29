using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class MsgEncrypted
    {
        public class Iv
        {
            public List<uint> data { get; set; }
        }

        public class EphemPublicKey
        {
            public List<uint> data { get; set; }
        }

        public class Ciphertext
        {
            public List<uint> data { get; set; }
        }

        public class Mac
        {
            public List<uint> data { get; set; }
        }

        public class Root
        {
            public Iv iv { get; set; }
            public EphemPublicKey ephemPublicKey { get; set; }
            public Ciphertext ciphertext { get; set; }
            public Mac mac { get; set; }
        }
    }
}
