using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Services.DataTypes
{
    public class ClientCryptoPack
    {
        public string KeyPath { get; set; }
        public string Xpub { get; set; }
        public string KeyShare {get; set;}
    }
}
