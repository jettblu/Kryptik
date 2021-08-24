using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class CurrencyWallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Xpub { get; set; }
        public string CoinId { get; set; }
        public string DepositAddress { get; set; }
        public string DepositQrBlockchain { get; set; }
        public string AccountId { get; set; }
        public double AccountBalanceFiat { get; set; }
        public double AccountBalanceCrypto { get; set; }
        public Wallet WalletKryptik { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationTime { get; set; }

    }
}
