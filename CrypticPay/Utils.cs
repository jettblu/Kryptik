using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CrypticPay.Data.Chart;

// IMPORTANT: Update so db context is inject as a dependency

namespace CrypticPay
{
    public static class Utils
    {
        private static List<string> _ValidExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
        private static CrypticPayContext _context;


        public static bool Requires2fa(CrypticPayUser user)
        {
            return (user.TwoFactorEnabled && user.PhoneNumberConfirmed && user.PhoneNumber != null);
        }
        public static bool IsValidPhoto(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return _ValidExtensions.Contains(extension.ToLower());
        }

        public static CrypticPayUser FindUserByMobile(string number, CrypticPayContext context)
        {
            _context = context;
            var customers = from m in _context.Users
                            select m;
            // take first user that has given phone number
            var user = customers.Where(s => s.PhoneNumber == number).FirstOrDefault();
            return user;
        }

        public static bool MobileAlreadyExists(string number, CrypticPayContext context)
        {
            _context = context;
            var customers = from m in _context.Users
                            select m;
            // take first user that has given phone number
            // return customers.Any(c => c.PhoneNumber == number);

            // uncomment line above when done debugging
            return false;

        }

        public static string SquishLink(string longLink)
        {
            Uri address = new Uri("http://tinyurl.com/api-create.php?url=" + longLink);
            System.Net.WebClient client = new System.Net.WebClient();
            string tinyUrl = client.DownloadString(address);
            return tinyUrl;
        }


        public class FriendAndFriendship {
            public CrypticPayUser Friend;
            public CrypticPayFriendship Friendship;
        }


        public static async Task<List<FriendAndFriendship>> GetPendingFriendshipsRecieved(CrypticPayFriendshipContext friendsContext, UserManager<CrypticPayUser> userManager, string id)
        {
            /*var friends = from f in friendsContext.Friends
                          join user in users on f.FriendTo equals user.Id
                          select f;*/

            var friends = from f in friendsContext.Friends
                          where (f.FriendTo == id && f.IsConfirmed == false)
                          select f;



            var pendingFriends = friends.ToList();

            var result = new List<FriendAndFriendship>();

            foreach(var friend in friends)
            {
                var friendInfo = new FriendAndFriendship()
                {
                    Friend = await userManager.FindByIdAsync(friend.FriendFrom),
                    Friendship = friend
                };
                result.Add(friendInfo);
            }

            return result;
        }

        public static List<CrypticPayFriendship> GetPendingFriendshipsSent(CrypticPayFriendshipContext friendsContext, string userId)
        {

            var friends = from f in friendsContext.Friends
                          where (f.FriendFrom == userId && f.IsConfirmed == false)
                          select f;



            var pendingFriends = friends.Where(f => f.IsConfirmed == false).ToList();


            return pendingFriends;
        }

        public static Globals.Status AddFriend(CrypticPayFriendshipContext friendsContext, string friendFromId, string friendToId)
        {
            var newFriend = new CrypticPayFriendship() { FriendFrom = friendFromId, FriendTo = friendToId, IsConfirmed = false, RequestTime = DateTime.Now };
            try
            { 
                friendsContext.Add(newFriend);
                friendsContext.SaveChanges();
                return Globals.Status.Success;
            }
            catch
            {
                return Globals.Status.Failure;
            }

        }
        
        // CHECK USERMANAGER DB PERFORMANCE
        public async static Task<Globals.Status> AcceptFriendAsync(CrypticPayFriendshipContext friendshipContext, UserManager<CrypticPayUser> userManager, string userId, string friendId)
        {
            var friends = from f in friendshipContext.Friends
                          where ((f.FriendFrom == userId || f.FriendFrom == friendId) && (f.FriendTo == userId || f.FriendTo == friendId))
                          select f;
            var friendship = friends.FirstOrDefault();
            friendship.IsConfirmed = true;
            friendship.BecameFriendsTime = DateTime.Now;

            var user = await userManager.FindByIdAsync(userId);
            var friend = await userManager.FindByIdAsync(friendId);

            friend.FriendCount += 1;
            user.FriendCount += 1;

           
            try
            {
                friendshipContext.SaveChanges();
                var result = await userManager.UpdateAsync(user);
                result = await userManager.UpdateAsync(friend);
                return Globals.Status.Success;
            }
            catch
            {
                return Globals.Status.Failure;
            }
            
        }

        // CHECK USERMANAGER DB PERFORMANCE
        public async static Task<Globals.Status> RejectFriendAsync(CrypticPayFriendshipContext friendshipContext, UserManager<CrypticPayUser> userManager, string userId, string friendId)
        {
            var friends = from f in friendshipContext.Friends
                          where ((f.FriendFrom == userId || f.FriendFrom == friendId) && (f.FriendTo == userId || f.FriendTo == friendId))
                          select f;
            var friendship = friends.FirstOrDefault();


            var user = await userManager.FindByIdAsync(userId);
            var friend = await userManager.FindByIdAsync(friendId);

            // decrement friend count if friendship has previously been confirmed
            if (friendship.IsConfirmed)
            {
                user.FriendCount -= 1;
                friend.FriendCount -= 1;
            }


            try
            {
                friendshipContext.Friends.Remove(friendship);
                friendshipContext.SaveChanges();
                var result = await userManager.UpdateAsync(user);
                result = await userManager.UpdateAsync(friend);
                return Globals.Status.Success;
            }
            catch
            {
                return Globals.Status.Failure;
            }

        }

        // removes all friendships a deleted user is involved with
        public async static Task<Globals.Status> RemoveUserFriendships(CrypticPayFriendshipContext friendshipContext, UserManager<CrypticPayUser> userManager, string userId)
        {
            var friends = from f in friendshipContext.Friends
                          where ((f.FriendFrom == userId || f.FriendTo == userId))
                          select f;


            var user = await userManager.FindByIdAsync(userId);



            try
            {   
                foreach(var friendship in friends)
                {
                    string friendId = "";
                    if(userId == friendship.FriendFrom)
                    {
                        friendId = friendship.FriendTo;
                    }
                    else
                    {
                        friendId = friendship.FriendFrom;
                    }
                    var friend = await userManager.FindByIdAsync(friendId);
                    friend.FriendCount -= 1;

                    friendshipContext.Friends.Remove(friendship);
                    friendshipContext.SaveChanges();
                    var result = await userManager.UpdateAsync(friend);
                    
                }
                return Globals.Status.Success;

            }
            catch
            {
                return Globals.Status.Failure;
            }

        }

        public static Globals.StatusFriend CheckFriendStatus(CrypticPayFriendshipContext friendsContext, string userId, string friendId)
        {
            var friends = from f in friendsContext.Friends
                          where ((f.FriendFrom == userId || f.FriendFrom == friendId) && (f.FriendTo == userId || f.FriendTo == friendId))
                          select f;        

            if(friends.Count() == 0)
            {
                return Globals.StatusFriend.NotFriends;
            }
            else
            {
                var friendship = friends.FirstOrDefault();
                if (friendship.IsConfirmed)
                {
                    return Globals.StatusFriend.ConfirmedFriends;
                }
                else
                {
                    return Globals.StatusFriend.PendingFriends;
                }
            }
            
        }

        public static CrypticPayFriendship GetFriendship(CrypticPayFriendshipContext friendsContext, string userId, string friendId)
        {
            var friends = from f in friendsContext.Friends
                          where ((f.FriendFrom == userId || f.FriendFrom == friendId) && (f.FriendTo == userId || f.FriendTo == friendId))
                          select f;
            return friends.FirstOrDefault();
        }

        public static DateTime GetLookBack(string query, CrypticPayCoins coin)
        {   
            // clean string in case contaminated on client side
            query = query.Trim().ToLower();
            var coinSpan = DateTime.Now - coin.DateCreated;
            // prevents requesting prices from when coin did not exist
            if(coinSpan.TotalDays < 90)
            {
                query = "all";
            }
            switch (query) 
            {
                case "1d":
                    return DateTime.Today.AddDays(-1);
                case "1w":
                    return DateTime.Today.AddDays(-7);
                case "1m":
                    return DateTime.Today.AddMonths(-1);
                case "3m":
                    return DateTime.Today.AddMonths(-3);
                case "1y":
                    return DateTime.Today.AddYears(-1);
                case "all":
                    return DateTime.Today.AddDays(-coinSpan.TotalDays);
                default:
                    return DateTime.Today.AddDays(-7);
            }

        }

        public static ChartData GetLabelsandData(string query, CoinData.CoinHistory coinHistory)
        {
            // clean string in case contaminated on client side
            query = query.Trim().ToLower();

            var timeStamps = new List<long>();
            var labelsFormatted = new List<string>();
            var prices = new List<double>();

            foreach (var data in coinHistory.prices)
            {
                timeStamps.Add(Convert.ToInt64(data[0]));
                // adjust rounded values based on price, less rounding for lower prices
                if(data[1] > 1)
                {
                    prices.Add(Math.Round(data[1], 2));
                }
                else if(data[1] < 1 && data[1] > .09)
                {
                    prices.Add(Math.Round(data[1], 3));
                }
                else
                {
                    prices.Add(Math.Round(data[1], 4));
                }
                
            }

            // change labels based on lookback
            switch (query)
            {
                case "1d":
                    foreach(var label in timeStamps)
                    {
                         string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortTimeString();
                         labelsFormatted.Add(stamp);
                    }
                    break;
                case "1w":
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.DayOfWeek.ToString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
                case "1m":
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortDateString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
                case "3m":
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortDateString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
                case "1y":
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortDateString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
                case "All":
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortDateString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
                default:
                    foreach (var label in timeStamps)
                    {
                        string stamp = DateTimeOffset.FromUnixTimeMilliseconds(label).DateTime.ToShortDateString();
                        labelsFormatted.Add(stamp);
                    }
                    break;
            }

            var result = new ChartData();
            result.Labels = labelsFormatted;
            result.Prices = prices;
            return result;

        }


        public static AssetData MakeChart(ChartData data, CrypticPayCoins coin)
        {   
            var currPrice = Math.Round(data.Prices.Last(), 2);
            var firstPrice = data.Prices[0];
            var priceDiffRaw =  Math.Round(currPrice - firstPrice, 2);
            var priceDiffPercent = Math.Round(100*((float)priceDiffRaw / (float)firstPrice), 2);

            string borderColor;

            var priceString = "";
            bool isGreen = true;

            if(priceDiffRaw > 0)
            {
                priceString = $"+${priceDiffRaw} (+{priceDiffPercent}%)";
                borderColor = "#00d150";
            }
            else if (priceDiffRaw < 0)
            {
                priceString = $"-${priceDiffRaw*-1} ({priceDiffPercent}%)";
                isGreen = false;
                borderColor = "#ff7070";
            }
            else
            {
                priceString = $"+${priceDiffRaw}";
                borderColor = "#007382";
            }

 


            // generic json to seed chart object
            var chartJson = $@" {{
            type: 'line',

            data: {{
                labels: { JsonConvert.SerializeObject(data.Labels)},
                datasets: [{{
                    label: 'Price',
                    backgroundColor: '#03dbfc',
                    borderColor: {JsonConvert.SerializeObject(borderColor)},
                    data: { JsonConvert.SerializeObject(data.Prices)},
                        pointRadius: 0,
                        pointBackgroundColor: 'white',
                        pointHoverBackgroundColor: 'white',
                        pointBorderColor: '#00c703',
                        pointHitRadius: 12
                }}]
            }},

            options: {{
              legend: {{
                    display: false
                 }},
              scales: {{
                xAxes: [{{
                    gridLines: {{
                        drawBorder: false,
                        display:false
                    }},

                    ticks: {{
                           display: false //this will remove only the label
                       }}

                }}],
                yAxes: [{{
                    gridLines: {{
                        drawBorder: false,
                        display:false
                        }}
                    }}]
                  }}
                }}
            }}";


            var chart = JsonConvert.DeserializeObject<Root>(chartJson);



            var result = new AssetData(chart, JsonConvert.SerializeObject(priceString), JsonConvert.SerializeObject(isGreen), currPrice.ToString(), coin);

            return result;
            
        }


        public static CrypticPayCoins FindCryptoByID(CrypticPayCoinContext contextCoins, string id)
        {
            var coins = from m in contextCoins.Coins
                        select m;

            // find crypto by api tag
            
            return coins.First(s => s.ApiTag == id);

        }


        public static string QrForWebGenerator(string toEncode)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(toEncode, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            // prepend html info
            return "data:image/png;base64," + qrCode.GetGraphic(20);
        }

        public static IQueryable<CrypticPayCoins> GetSupportedCoins(CrypticPayCoinContext contextCoins)
        {
            var coins = from c in contextCoins.Coins
                          where ((c.IsSupported == true))
                          select c;
            return coins;
        }

        public static string FindCryptoIdByName(string name, CrypticPayCoinContext contextCoins)
        {
            var coins = from m in contextCoins.Coins
                        select m;

            // find crypto by api tag

            return coins.First(m => m.Name == name).Id;
        }


        // gets coin object corresponding to each crypto in kryptik wallet
        public static WalletHandler.WalletandCoins GetCoinsForWallet(CrypticPayUser user, CrypticPayCoinContext contextCoins)
        {
            var walletCoinContainer = new WalletHandler.WalletandCoins();
            walletCoinContainer.User = user;
            walletCoinContainer.Coins = new List<CrypticPayCoins>();
            foreach (var account in user.WalletKryptik.CurrencyWallets)
            {
                var coin = contextCoins.Coins.Find(account.CoinId);
                walletCoinContainer.Coins.Add(coin);
            }
            return walletCoinContainer;
        }


        public static List<CrypticPayUser> SearchUsers(CrypticPayUser user, CrypticPayContext context, string query, int amount=0)
        {
            var customers = from m in context.Users
                            select m;

            // uncomment below to return page if search bypasses client side checks and is empty

            /*if (string.IsNullOrEmpty(query))
            {
                StatusMessage = "Please enter a valid query.";
                return Page();
            }*/



            // match customer name, username, or number based on query. Exclude current user from result
            return customers.Where(s => (s.UserName.Contains(query) || s.PhoneNumber.StartsWith(query) || s.Name.Contains(query)) && s.UserName != user.UserName).ToList();
        }

        

    }

}


