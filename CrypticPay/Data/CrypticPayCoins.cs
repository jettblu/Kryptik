using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class CrypticPayCoins
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiTag { get; set; }
        public string Ticker { get; set; }
        public string ThumbnailPath { get; set; }
        public string Color { get; set; }
        public bool IsSupported { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
    }
}
