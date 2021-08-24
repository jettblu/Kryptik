using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinGecko;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrypticPay.Data
{
    public class CoinData
    {
        public class CoinHistory
        {
            public List<List<double>> prices { get; set; }
            public List<List<object>> market_caps { get; set; }
            public List<List<double>> total_volumes { get; set; }
        }

        public class Bitcoin
        {
            public int usd { get; set; }
        }

        public class Ethereum
        {
            public double usd { get; set; }
        }

        public class Litecoin
        {
            public double usd { get; set; }
        }

        public class BitcoinCash
        {
            public double usd { get; set; }
        }

        public class CoinHolder
        {
            public Bitcoin bitcoin { get; set; }
            public Ethereum ethereum { get; set; }
            public Litecoin litecoin { get; set; }

            [JsonProperty("bitcoin-cash")]
            public BitcoinCash BitcoinCash { get; set; }
        }



        public static CoinHistory GetCoinHistory(string coinID, DateTime start)
        {   
            var epochBase = new DateTime(1970, 1, 1);
            var startTime = (start - epochBase).TotalSeconds;
            var endTime = (DateTime.Now - epochBase).TotalSeconds;
            var query = $"https://api.coingecko.com/api/v3/coins/{coinID}/market_chart/range?vs_currency=usd&from={startTime}&to={endTime}";
                     
            string json;

            using (var web = new System.Net.WebClient())
            {
                var url = query;
                json = web.DownloadString(url);
            }

            var coinHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<CoinHistory>(json);

            return coinHistory;
        }

        public static List<Double> GetCoinsExchangeRates(List<CrypticPayCoins> coins)
        {
            string coinString = "";
            foreach (var coin in coins)
            {
                coinString += coin.ApiTag + ",";
            };

            //remove last comma
            coinString = coinString.Remove(coinString.Length - 1);

            var query = $"https://api.coingecko.com/api/v3/simple/price?ids={coinString}&vs_currencies=USD&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";

            string json;

            using (var web = new System.Net.WebClient())
            {
                var url = query;
                json = web.DownloadString(url);
            }

            var jsonParsed = JObject.Parse(json);

            var result = new List<double>();
            foreach (var item in jsonParsed)
            {
                result.Add(Convert.ToDouble(item.Value.First.First));
            }
            return result;
        }
    }
}
