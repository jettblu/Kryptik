using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class BlockchainAddress
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public int Index { get; set; }
        public string Address { get; set; }
        public string XpubMaster { get; set; }
        public DateTime DateCreated;
        
        public CurrencyWallet CurrencyWallet { get; set; }
    }
}
