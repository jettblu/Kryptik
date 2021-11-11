using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Services.Interfaces
{
    interface IChatHandler
    {
        public class GroupAndMembers
        {
            public Data.Group Group { get; set; }
            public IEnumerable<string> UserIds { get; set; }
        }

        public GroupAndMembers CreateGroup(Areas.Identity.Data.CrypticPayUser creator, List<string> memberIds, bool isPrivate = true);
    }
}
