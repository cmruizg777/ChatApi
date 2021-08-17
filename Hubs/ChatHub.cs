using ChatApi.DTOs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MensajeDto newMessage)
        {
            await Clients.All.SendAsync("ReceiveMessage", newMessage);
        }
    }
}
