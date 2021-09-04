using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Data;
using Microsoft.AspNetCore.Identity;

namespace CrypticPay.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CrypticPayUser class
    public class CrypticPayUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string ProfilePhotoPath { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }
        [PersonalData]
        public int FriendCount { get; set; }
        public bool WalletKryptikExists { get; set; }
        public Wallet WalletKryptik { get; set; }
    }
}
