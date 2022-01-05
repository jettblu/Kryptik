using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CrypticPay.Areas.Payments.Pages
{
    public class TestModel : PageModel
    {   
        public string Message { get; set; }
        private readonly CrypticPayCoinContext _contextCoins;
        private readonly CrypticPayContext _contextUsers;
        private WalletHandler _walletHandler;
        private UserManager<CrypticPayUser> _userManager;

        public TestModel(CrypticPayContext context, CrypticPayCoinContext contextCoins, WalletHandler walletHandler, UserManager<CrypticPayUser> userManager)
        {
            _contextUsers = context;
            _contextCoins = contextCoins;
            _walletHandler = walletHandler;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async void OnPostTestAsync()
        {
            Message = "Testing";
            var userId = _userManager.GetUserId(User);
            var coin = Utils.FindCryptoByTicker("Ltc", _contextCoins);
            var transactions = await _walletHandler.GetTransactions(userId, _contextUsers, coin);
        }
    }
}
