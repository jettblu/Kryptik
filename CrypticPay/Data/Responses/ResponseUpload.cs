using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data.Responses
{
    public class ResponseUpload
    {
        public Globals.Status Status { get; set; }
        public string CID { get; set; }
    }
}
