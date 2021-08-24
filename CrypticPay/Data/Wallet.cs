using CrypticPay.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class Wallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public List<CurrencyWallet> CurrencyWallets { get; set; }
        public byte [] Phrase { get; set; }
        public byte[] Decrypter { get; set; }
        public byte[] Iv { get; set; }
        public bool IsCustodial { get; set; }
        public string CrypticPayUserKey { get; set; }
        public CrypticPayUser Owner { get; set; }
        public double BalanceTotal { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationTime { get; set; }
    }
}
