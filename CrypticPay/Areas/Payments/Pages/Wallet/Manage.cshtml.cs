using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [DataType(DataType.Password)]
            public string Password { get; set; }

        }


        public void OnGet()
        {
            LoadUserData();
        }

        public void LoadUserData()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CurrUser = _walletHandler.GetUserandWallet(userId, _context);
        }


        public async Task<IActionResult> OnPostCreateWalletAsync()
        {
            Globals.Status walletCreationStatus;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUser = _walletHandler.GetUserandWallet(userId, _context);
              // wait for wallet to be created
                var response = await _walletHandler.CreateWallet(currUser, _contextCoins);
                // Ensure user's wallet changes are saved
                currUser.WalletKryptikExists = true;
                await _userManager.UpdateAsync(currUser);
                walletCreationStatus = Globals.Status.Success;
            

            return new PartialViewResult()
            {
                ViewName = "_WalletCreation",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = walletCreationStatus
                }
            };

        }

        public async Task<PageResult> OnPostDeleteWallet()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUser = _walletHandler.GetUserandWallet(userId, _context);
            // reset propert for page load
            CurrUser = currUser;
            var pwordCheck = await _userManager.CheckPasswordAsync(currUser, Input.Password);

            if (!pwordCheck)
            {
                StatusMessage = "Error: Invalid Password";
                
                return Page();
            }
            try
            {
                if(currUser.WalletKryptikExists && currUser.WalletKryptik != null)
                     {  
                        
                         _context.Remove(currUser.WalletKryptik);
                         currUser.WalletKryptikExists = false;
                         _context.SaveChanges();
                         StatusMessage = "Wallet deleted.";
                      }
            }
            catch
            {
                StatusMessage = "Error occured while deleting wallet.";
            }

            LoadUserData();
            return Page();
        }

        }
}
