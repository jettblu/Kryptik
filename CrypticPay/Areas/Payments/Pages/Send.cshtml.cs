using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CrypticPay.Areas.Payments
{
    public class SendModel : PageModel
    {
        private readonly CrypticPayCoinContext _contextCoins;

        public SendModel(CrypticPayCoinContext context)
        {

            _contextCoins = context;

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
    }
}
