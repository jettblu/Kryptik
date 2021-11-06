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
        public ChatHub(UserManager<CrypticPayUser> userManager)
        {
            _userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Groups.AddToGroupAsync(Context.ConnectionId, userId);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public Task SendMessageToGroup(string receiver, string message)
        {
            var sender = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }
    }
}
