using CrypticPay.Areas.Identity.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CrypticPay.Areas.Community
{
    public partial class SearchModel : PageModel
    {

        private readonly Data.CrypticPayContext _context;
        private readonly UserManager<CrypticPayUser> _userManager;
        private WalletHandler _walletHandler;
        private Data.CrypticPayCoinContext _contextCoins;

        public SearchModel(Data.CrypticPayContext context, Data.CrypticPayCoinContext contextCoins, UserManager<CrypticPayUser> userManager, WalletHandler walletHandler)
        {
            _contextCoins = contextCoins;
            _context = context;
            _userManager = userManager;
            _walletHandler = walletHandler;
        }

        // initialize as empty list to avoid errors in view
        public IList<CrypticPayUser> Users = new List<CrypticPayUser>();


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; }
        }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            
        }

        public async Task<JsonResult> OnPostSearchAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var query = Input.SearchString;


            // uncomment below to return page if search bypasses client side checks and is empty

            /*if (string.IsNullOrEmpty(query))
            {
                StatusMessage = "Please enter a valid query.";
                return Page();
            }*/



            // match customer name, username, or number based on query
            Users = Utils.SearchUsers(user, _context, query);
            return new JsonResult(Users);
        }

        public async Task OnPostTestAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _walletHandler.GetUserandWallet(userId, _context);
            var coin = Utils.FindCryptoByTicker("BTC", _contextCoins);
            var currWallet = _walletHandler.GetCurrencyWallet(coin, user);
            _walletHandler.CreateAddress(currWallet, coin);
            
            /*var mnemonic = _walletHandler.DecryptMnemonic(user);*/
            /*var resultv = await _walletHandler.MatchDepositAddresses(userId, _context);*/
            /*var coin = Utils.FindCryptoByTicker("LTC", _contextCoins);
            var transactions = await _walletHandler.GetTransactions(userId, _context, coin);*/
            /*var currUser = await _userManager.GetUserAsync(User);
            // wait for wallet to be created
            var response = await _walletHandler.CreateWallet(currUser, _contextCoins);
            // Ensure user's wallet changes are saved
            currUser.Approved = true;
            _context.Users.Update(currUser);
            _context.SaveChanges();
            // decrypt mnemonic
            var phrase = _walletHandler.DecryptMnemonic(currUser);*/
        }

    }
}
