using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CrypticPay.Hubs
{
    public class ChatHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public Task SendMessageToGroup(string receiver, string message)
        {
            var sender = Context.User.Identity.Name;
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }
    }
}
