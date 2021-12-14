using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static CrypticPay.Utils;

namespace CrypticPay.Areas.Identity.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly CrypticPayContext _context;
        private readonly CrypticPayFriendshipContext _contextFriends;
        private readonly UserManager<CrypticPayUser> _userManager;

        


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IQueryable<FileUpload> Uploads { get; set; }
        public string FriendOnGetInfo { get; set; }
        public FriendRequestData FriendRequestInfo { get; set; }
        public class FriendRequestData
        {
            public IList<FriendAndFriendship> FriendsPending { get; set; }
            public string NotificationMessage { get; set; }
            public int FriendCount { get; set; }
        }
        


        public class InputModel
        {
            public bool FriendAccepted { get; set; }
            public string FriendName { get; set; }
            public string Id { get; set; }
        }

        public ProfileModel(UserManager<CrypticPayUser> userManager, CrypticPayFriendshipContext contextFriends, WalletHandler walletHandler, CrypticPayContext contextUsers)
        {
            _userManager = userManager;
            _contextFriends = contextFriends;
            _userManager = userManager;
            _context = contextUsers;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            FriendRequestInfo = new FriendRequestData() { FriendsPending = await Utils.GetPendingFriendshipsRecieved(_contextFriends, _userManager,currUser.Id)};
            Input = new InputModel { Id = currUser.Id };
            FriendOnGetInfo = JsonConvert.SerializeObject(FriendRequestInfo);
            // retreive current users
            Uploads = Utils.GetUploads(_context, currUser);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateFriendAsync()
        {
            var result = Globals.Status.Pending;
            var friendUser = await _userManager.FindByNameAsync(Input.FriendName);
            string message;
            if (Input.FriendAccepted)
            {
                result = await Utils.AcceptFriendAsync(_contextFriends, _userManager, Input.Id, friendUser.Id);
            }
            else
            {
                result = await Utils.RejectFriendAsync(_contextFriends, _userManager, Input.Id, friendUser.Id);
            }

            if (result == Globals.Status.Failure)
            {
                message = "Error: Issue handling friend request.";
            }
            else
            {
                message = "Friend Added.";
            }

            var currUser = await _userManager.FindByIdAsync(Input.Id);
            var friendCount = currUser.FriendCount;

            FriendRequestInfo = new FriendRequestData() { FriendsPending = await Utils.GetPendingFriendshipsRecieved(_contextFriends, _userManager, Input.Id), NotificationMessage = message, FriendCount =  friendCount};

            return new JsonResult(FriendRequestInfo);
        }


    }
}
