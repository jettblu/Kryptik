using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CrypticPay.Areas.Payments.Pages.Wallet
{
    public class ManageModel : PageModel
    {

        private readonly Data.CrypticPayContext _context;
        private readonly UserManager<CrypticPayUser> _userManager;
        private WalletHandler _walletHandler;
        private Data.CrypticPayCoinContext _contextCoins;
        public CrypticPayUser CurrUser { get; set; }

        public ManageModel(Data.CrypticPayContext context, Data.CrypticPayCoinContext contextCoins, UserManager<CrypticPayUser> userManager, WalletHandler walletHandler)
        {
            _contextCoins = contextCoins;
            _context = context;
            _userManager = userManager;
            _walletHandler = walletHandler;
        }

        
        public void OnGet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CurrUser = _walletHandler.GetUserandWallet(userId, _context);
        }


        public async Task<IActionResult> OnPostCreateWalletAsync()
        {
            Globals.Status walletCreationStatus;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUser = _walletHandler.GetUserandWallet(userId, _context);
            try
            {                // wait for wallet to be created
                var response = await _walletHandler.CreateWallet(currUser, _contextCoins);
                // Ensure user's wallet changes are saved
                currUser.WalletKryptikExists = true;
                await _userManager.UpdateAsync(currUser);
                walletCreationStatus = Globals.Status.Success;
            }
            catch
            {
                walletCreationStatus = Globals.Status.Failure;
            }

            return new PartialViewResult()
            {
                ViewName = "_WalletCreation",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = walletCreationStatus
                }
            };

        }
    }
}
