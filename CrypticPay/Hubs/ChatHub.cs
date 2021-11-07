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
        public Task SendMessageToGroup(string receiver, string message)
        {
            var sender = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.Find(sender);
            var uName = user.UserName;
            return Clients.Group(receiver).SendAsync("ReceiveMessage", uName, message);
        }
        /*Make sure messages are placed into correct view on client side and add sidebar view*/
    }
}
