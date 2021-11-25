using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Services.DataTypes
{
    public class GroupAndMembers
    {
        public Data.Group Group { get; set; }
        public IEnumerable<string> UserIds { get; set; }
        public IEnumerable<Data.ChatData> Messages { get; set; }
        public byte [] RecipientKey { get; set; }
    }
}
