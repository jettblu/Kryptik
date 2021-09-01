using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Newtonsoft.Json;

namespace CrypticPay.Areas.Payments
{
    public class SendModel : PageModel
    {
        private readonly CrypticPayCoinContext _contextCoins;
        private readonly Data.CrypticPayContext _context;
        private readonly UserManager<CrypticPayUser> _userManager;
        private WalletHandler _walletHandler;


        public SendModel(CrypticPayCoinContext contextCoins, CrypticPayContext contextUsers, UserManager<CrypticPayUser> userManager, WalletHandler walletHandler)
        {

            _contextCoins = contextCoins;
            _context = contextUsers;
            _userManager = userManager;
            _walletHandler = walletHandler;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; }
            public string SearchStringAsync { get; set; }
            [Required]
            public string To { get; set; }
            [Required]
            public string For { get; set; }
            [Required]
            public string CoinName { get; set; }
        }

        public Dictionary<string, string> Data { get; set; }
        public string DataJson { get; set; }

        public void OnGet()
        {
            Data = new Dictionary<string, string>();
            var coins = _contextCoins.Coins;
            foreach(var coin in coins)
            {
                Data[coin.Name] = coin.ThumbnailPath;
            }

            DataJson = JsonConvert.SerializeObject(Data);
        }


        public async Task<IActionResult> OnPostSearchUsers()
        {

            var user = await _userManager.GetUserAsync(User);
            var query = Input.SearchStringAsync;


            // uncomment below to return page if search bypasses client side checks and is empty

            /*if (string.IsNullOrEmpty(query))
            {
                StatusMessage = "Please enter a valid query.";
                return Page();
            }*/



            // match customer name, username, or number based on query
            var users = Utils.SearchUsers(user, _context, query);

            return new PartialViewResult()
            {
                ViewName = "_userSearchResult",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = users
                }
            };
        }

        public void OnPostCreateTransaction()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUser = _walletHandler.GetUserandWallet(userId, _context);
            var coinToSend = Utils.FindCryptoIdByName(Input.CoinName, _contextCoins);

            
            

        }

        
    }
}
