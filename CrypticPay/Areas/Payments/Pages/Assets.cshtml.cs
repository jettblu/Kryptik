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
using Newtonsoft.Json;
using static CrypticPay.Data.Chart;

namespace CrypticPay.Areas.Payments
{
    public class AssetsModel : PageModel
    {

        private readonly CrypticPayCoinContext _contextCoins;

        public AssetsModel(CrypticPayCoinContext context)
        {
            _contextCoins = context;
        }

        [BindProperty]
        public string AssetId { get; set; }

        public WalletHandler.WalletandCoins WalletandInfo {get; set;}

        public string JsonChart { get; set; }
        public AssetData _assetData { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string LookBack { get; set; }
        }
        public void OnGet()
        {
            AssetId = Request.Query["cname"];
            // default to bitcoin if asset not specified on get
            if (String.IsNullOrEmpty(AssetId))
            {   
                AssetId = "bitcoin";
            }         

            MakeChart("1w");


        }

        public void MakeChart(string lookBack)
        {   
            var coin = Utils.FindCryptoByID(_contextCoins, AssetId);
            var startDate = Utils.GetLookBack(lookBack, coin);
            var lookBackData = CoinData.GetCoinHistory(coinID: coin.ApiTag, start: startDate);
            var chartData = Utils.GetLabelsandData(query: lookBack, coinHistory: lookBackData);
            _assetData = Utils.MakeChart(chartData, coin);
            JsonChart = JsonConvert.SerializeObject(_assetData.ChartJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public JsonResult OnPostUpdateChart()
        {   

            MakeChart(Input.LookBack);        
            return new JsonResult(_assetData);
        }



    }
}
