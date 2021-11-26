using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Services;

namespace CrypticPay.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private Data.CrypticPayContext _context;
        private ChatHandler _chatter;
        private Crypto _crypto;
        private WalletHandler _walletHandler;


        public ChatHub(UserManager<CrypticPayUser> userManager, Data.CrypticPayContext context, ChatHandler chatHandler, Crypto crypto, WalletHandler walletHandler)
        {
            _userManager = userManager;
            _context = context;
            _chatter = chatHandler;
            _crypto = crypto;
            _walletHandler = walletHandler;
        }
        // UPDATE TO SUPPORT >2 GROUPS
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _walletHandler.GetUserandWallet(userId, _context);
            var uName = user.UserName;
            Groups.AddToGroupAsync(Context.ConnectionId, uName);
            // initialize values on client side for encryption
            var userAndCrypto = _crypto.GetClientCrypto(user);
            Clients.Group(uName).SendAsync("SetCrypto", userAndCrypto.KeyPath, userAndCrypto.KeyShare);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        

        // UPDATE THIS TO SUPPORT >2 GROUPS. MAYBE SEARCH GROUP AND BROADCAST TO ALL MEMBERS.
        public Task SendMessageToGroup(string receiver, string message, string messageSender, string messageReciever, string groupId)
        {
            var sender = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.Find(sender);
            var uName = user.UserName;
            // save message
            var msg = new Data.ChatData()
            {
                GroupId = groupId,
                Message = message,
                MessageFrom = messageSender,
                MessageTo = messageReciever,
                SenderId = user.Id,
                IsRead = false,
                CreationTime = DateTime.Now
            };
            // generate chat hub partial
            var grouper = new Services.DataTypes.GroupAndMembers()
            {
                Group = _context.Groups.Find(groupId),
                // eventually add all members
                UserIds = _chatter.GroupMembers(groupId)
            };
            _context.Chats.Add(msg);
            _context.SaveChanges();
            string sideBox = CreateSideBox(grouper, user);
            // FIX GROUP NAME 
            return Clients.Group(receiver).SendAsync("ReceiveMessage", uName, message, groupId, sideBox);
        }
        // MAKE SURE MESSAGES ARE IN CORRECT VIEW AND SIDEBAR IS UPDATED ON CLIENT

        // create sideBox string to display on client
        public string CreateSideBox(Services.DataTypes.GroupAndMembers gu, CrypticPayUser currUser )
        {
            var nameList = new List<string>();
            var photoList = new List<string>();
            var uList = new List<string>();
            foreach (var userId in gu.UserIds)
            {
                var user = _context.Users.Find(userId);
                if (userId != currUser.Id)
                {
                    nameList.Add(user.Name);
                    photoList.Add(user.ProfilePhotoPath);
                    uList.Add(user.UserName);
                }
            }
            string nameString = string.Join(",", nameList);
            string uNameString = string.Join(",", uList);

            var sideBoxHtml = $@"
                <div class=""row valign-wrapper msgSideBox"" data-group=""{gu.Group.Id}"" data-members=""{uNameString}"" data-nameTitle=""{nameString}"">
                <div class=""col s2"">
                    <img src = ""{photoList[0]}"" alt=""user photo"" style=""width: 40px;"" class=""circle"">
                </div>
                <div class=""col s9 offset-s1"">
                    <span style = ""align: left; font-size: 14px; font-weight: 400px;"" class=""truncate"">{nameString}</span>
                </div>
            </div>
            ";
            return sideBoxHtml;
        }

        

    }
}
