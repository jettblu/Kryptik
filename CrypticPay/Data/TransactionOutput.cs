using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class TransactionOutput
    {
        public uint256 TransactionId { get; set; }
        public uint OutputIndex { get; set; }
        public string AddressTo { get; set; }
        public uint256 TransactionHash { get; set; }
        public decimal Amount { get; set; }
        public string Fee { get; set; }
    }
}
