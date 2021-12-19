using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CrypticPay.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace CrypticPay.Areas.Payments.Pages.Wallet
{
    public class IndexModel : PageModel
    {
        private readonly CrypticPayCoinContext _contextCoins;
        private readonly CrypticPayContext _contextUsers;
        private WalletHandler _walletHandler;

        public IndexModel(CrypticPayContext context, CrypticPayCoinContext contextCoins, WalletHandler walletHandler)
        {
            _contextUsers = context;
            _contextCoins = contextCoins;
            _walletHandler = walletHandler;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostObtainWalletDataAsync()
        {
            // get current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // load relational data for user
            var currUser = _walletHandler.GetUserandWallet(userId, _contextUsers);
            var walletCoinContainer = Utils.GetCoinsForWallet(currUser, _contextCoins);
            if (currUser.WalletKryptikExists && currUser.WalletKryptik != null)
            {
                var resultBalUpdate = await _walletHandler.UpdateBalances(walletCoinContainer);
                // update user after updating balances
                _contextUsers.Users.Update(currUser);
                _contextUsers.SaveChanges();
            }

            
            var balancesPartial = new PartialViewResult()
            {
                ViewName = "_Balance",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = walletCoinContainer
                }
            };
            

            return balancesPartial;
        }
    }
}
