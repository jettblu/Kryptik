using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay
{
    public class TwilioVerifySettings
    {
        public string VerificationServiceSID { get; set; }
        public string AccountSID { get; set; }
        public string SendNumber { get; set; }
        public string AuthToken { get; set; }
    }
}
