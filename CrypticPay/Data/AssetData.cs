using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CrypticPay.Data.Chart;

namespace CrypticPay.Data
{
    public class AssetData
    {
        public AssetData(Root chartJson, string priceString, string isGreen, string currPrice, CrypticPayCoins coin)
        {
            ChartJson = chartJson;
            PriceString = priceString;
            IsGreen = isGreen;
            CurrentPrice = currPrice;
            Coin = coin;
        }

        public Root ChartJson { get; set; }
        public string PriceString { get; set; }
        public string CurrentPrice { get; set; }
        public string IsGreen { get; set; }
        public CrypticPayCoins Coin { get; set; }
    }
}
