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
        private readonly CrypticPayFriendshipContext _contextFriends;
        private readonly UserManager<CrypticPayUser> _userManager;

        private readonly CrypticPayCoinContext _contextCoins;
        private readonly CrypticPayContext _contextUsers;
        private WalletHandler _walletHandler;


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        
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

        public ProfileModel(UserManager<CrypticPayUser> userManager, CrypticPayFriendshipContext contextFriends, CrypticPayCoinContext context, WalletHandler walletHandler, CrypticPayContext contextUsers)
        {
            _userManager = userManager;
            _contextFriends = contextFriends;
            _contextUsers = contextUsers;
            _contextCoins = context;
            _userManager = userManager;
            _walletHandler = walletHandler;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            FriendRequestInfo = new FriendRequestData() { FriendsPending = await Utils.GetPendingFriendshipsRecieved(_contextFriends, _userManager,currUser.Id)};
            Input = new InputModel { Id = currUser.Id };
            FriendOnGetInfo = JsonConvert.SerializeObject(FriendRequestInfo);
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


        public async Task<IActionResult> OnPostObtainWalletDataAsync()
        {
            // get current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // load relational data for user
            var currUser = _contextUsers.Users.Include(us => us.WalletKryptik).ThenInclude(w => w.CurrencyWallets).Where(us => us.Id == userId).FirstOrDefault();
            var walletCoinContainer = Utils.GetCoinsForWallet(currUser, _contextCoins);
            if(currUser.WalletKryptik != null)
            {
                var resultBalUpdate = await _walletHandler.UpdateBalances(walletCoinContainer);
                // update user after updating balances
                _contextUsers.Users.Update(currUser);
                _contextUsers.SaveChanges();
            }
           

            return new PartialViewResult()
            {
                ViewName = "_Balance",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = walletCoinContainer
                }
            };
        }
    }
}
