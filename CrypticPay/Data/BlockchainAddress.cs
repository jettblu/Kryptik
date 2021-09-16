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
        public int Index;
        public string Address;
        public DateTime DateCreated;
        
        public CurrencyWallet CurrencyWallet { get; set; }
    }
}
