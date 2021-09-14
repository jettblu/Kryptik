using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay
{
    public static class Globals
    {
        public enum Status
        {
            Success = 0,
            Failure = 1,
            Pending = 2,
            Done = 3
        }

        public enum StatusFriend 
        {
            ConfirmedFriends = 0,
            PendingFriends = 1,
            NotFriends = 2,
            BlockedFriends = 3
        }

        public enum Roles {
            SuperAdmin,
            Admin,
            Basic
        }


    }
}
