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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CrypticPay.Areas.Payments.Pages.Coins
{
    public class FindModel : PageModel
    {

        private readonly Data.CrypticPayCoinContext _contextCoins;
  

        public FindModel(CrypticPayCoinContext context)
        {

            _contextCoins = context;

        }

        [TempData]
        public string StatusMessage { get; set; }

        public class JsonSearchresult
        {
            public List<Data.CrypticPayCoins> Coins { get; set; }
            public int CoinCount { get; set; }
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string SearchString { get; set; }
        }


        public  void OnGet()
        {

        }

        public JsonResult OnPostSearch()
        {

            var query = Input.SearchString;
            var coins = from m in _contextCoins.Coins
                        select m;

            var coinsSearched = new List<Data.CrypticPayCoins>();

            // uncomment below to return page if search bypasses client side checks and is empty

            /*if (string.IsNullOrEmpty(query))
            {
                StatusMessage = "Please enter a valid query.";
                return Page();
            }*/



            // match coin name or ticker based on query

            coinsSearched = coins.Where(s => s.Name.Contains(query) || s.Ticker.Contains(query)).ToList();

            var result = new JsonSearchresult() { CoinCount = coinsSearched.Count, Coins = coinsSearched };

            return new JsonResult(result);


        }
    }
}
