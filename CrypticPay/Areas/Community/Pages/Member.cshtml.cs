using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrypticPay.Areas.Community.Pages
{
    public class MemberModel : PageModel
    {
        private readonly Data.CrypticPayFriendshipContext _contextFriends;
        private readonly UserManager<CrypticPayUser> _userManager;
        public string PhotoPath { get; set; }
        public string PartnerName { get; set; }
        public string PartnerTag { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        
        public CrypticPayFriendship Friendship { get; set; }
        public CrypticPayUser ThisPartner { get; set; }
        public FriendData FriendInfo { get; set; }
        public class FriendData
        {
            public int FriendStatus { get; set; }
            public double FriendshipLengthHours { get; set; }
            public double FriendshipLengthDays { get; set; }
            public string NotificationMessage { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {

            public string UserName { get; set; }
            public Globals.StatusFriend FriendStatus {get; set;}
            public bool AddFriend { get; set; }
            public string Test { get; set; }
        }

        public MemberModel(UserManager<CrypticPayUser> userManager, Data.CrypticPayFriendshipContext contextFriends )
        {
            _userManager = userManager;
            _contextFriends = contextFriends;
        }

        // add error handling for blank request
        public async Task OnGetAsync()
        {
            var userName = Request.Query["uname"];

            try
            {
                ThisPartner = await _userManager.FindByNameAsync(userName);
            }
            catch
            {
                ThisPartner = await _userManager.FindByNameAsync(Input.UserName);
            }
            
            var currUser = await _userManager.GetUserAsync(User);
            PhotoPath = ThisPartner.ProfilePhotoPath;
            PartnerName = ThisPartner.Name;
            PartnerTag = ThisPartner.UserName;

            var friendStatus = Utils.CheckFriendStatus(_contextFriends, currUser.Id, ThisPartner.Id);


            if (friendStatus == Globals.StatusFriend.ConfirmedFriends)
            {
                try
                {
                    Friendship = Utils.GetFriendship(_contextFriends, currUser.Id, ThisPartner.Id);
                }
                catch
                {
                    throw new Exception("Unable to retrieve friendship");
                }
                
            }

            double friendshipLengthDays = 0;
            double friendshipLengthHours = 0;

            if (Friendship != null)
            {
                friendshipLengthDays = Math.Round((DateTime.Now - Friendship.BecameFriendsTime).TotalDays);
                friendshipLengthHours = Math.Round((DateTime.Now - Friendship.BecameFriendsTime).TotalHours);
            }


            FriendInfo = new FriendData() { FriendStatus = (int) friendStatus, FriendshipLengthDays =  friendshipLengthDays, FriendshipLengthHours = friendshipLengthHours};


            Input = new InputModel
            {
                UserName = PartnerTag,
                FriendStatus = friendStatus
            };


        }


        public async Task<JsonResult> OnPostManageFriendAsync()
        {
            var userFrom = _userManager.GetUserAsync(User).Result;
            var userTo = await _userManager.FindByNameAsync(Input.UserName);

            Globals.StatusFriend friendStatus;
            string message;

            // add friend on user request
            if (Input.AddFriend)
            {
                var update = Utils.AddFriend(_contextFriends, friendFromId: userFrom.Id, friendToId: userTo.Id);
                if (update == Globals.Status.Success)
                {
                    message = "Friend request sent";
                    friendStatus = Globals.StatusFriend.PendingFriends;
                }
                else
                {
                    message = "Error: Issue sending friend request";
                    friendStatus = Globals.StatusFriend.NotFriends;
                }
            }
            // reject friend on user request
            else
            {
                var update = await Utils.RejectFriendAsync(_contextFriends, _userManager, userId: userFrom.Id, friendId: userTo.Id);

                if (update == Globals.Status.Success)
                {
                    message = "Friend removed";
                    friendStatus = Globals.StatusFriend.NotFriends;
                }
                else
                {
                    message = "Error: Issue removing friend";
                    friendStatus = Globals.StatusFriend.PendingFriends;
                }
            }

            double friendshipLengthDays = 0;
            double friendshipLengthHours = 0;


            if (Friendship != null)
            {
                friendshipLengthDays = Math.Round((DateTime.Now - Friendship.BecameFriendsTime).TotalDays);
                friendshipLengthHours = Math.Round((DateTime.Now - Friendship.BecameFriendsTime).TotalHours);
            }



            FriendInfo = new FriendData() { FriendStatus = (int) friendStatus, FriendshipLengthDays = friendshipLengthDays, FriendshipLengthHours = friendshipLengthHours, NotificationMessage = message };

            return new JsonResult(FriendInfo);

        }
    }
}
