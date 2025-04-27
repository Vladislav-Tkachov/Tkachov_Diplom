using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Diplom.Client.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string ticketId, string user, string message)
        {
            await Clients.Group(ticketId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinTicketRoom(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);
        }

        public async Task LeaveTicketRoom(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);
        }
    }
}