using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CrypticPay.Areas.Identity.Data;

namespace CrypticPay.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private Data.CrypticPayContext _context;
        
        public ChatHub(UserManager<CrypticPayUser> userManager, Data.CrypticPayContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // UPDATE TO SUPPORT >2 GROUPS
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.Find(userId);
            var uName = user.UserName;
            Groups.AddToGroupAsync(Context.ConnectionId, uName);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        // UPDATE THIS TO SUPPORT >2 GROUPS. MAYBE SEARCH GROUP AND BROADCAST TO ALL MEMBERS.
        public Task SendMessageToGroup(string receiver, string message, string groupId)
        {
            var sender = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.Find(sender);
            var uName = user.UserName;
            // save message
            var msg = new Data.ChatData()
            {
                GroupId = groupId,
                Message = message,
                SenderId = user.Id,
                IsRead = false,
                CreationTime = DateTime.Now
            };
            _context.Chats.Add(msg);
            _context.SaveChanges();
            // FIX GROUP NAME 
            return Clients.Group(receiver).SendAsync("ReceiveMessage", uName, message, groupId);
        }
        // MAKE SURE MESSAGES ARE IN CORRECT VIEW AND SIDEBAR IS UPDATED ON CLIENT
    }
}
