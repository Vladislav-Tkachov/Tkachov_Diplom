using Diplom.Client.Server.Data;
using Diplom.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Diplom.Client.Server.Hubs
{
    [Authorize]
    public class ChatHub(ApplicationDbContext db) : Hub
    {
        public async Task SendMessage(string ticketId, string user, string message)
        {
            var parsedId = int.Parse(ticketId);

            var chatMessage = new ChatMessage
            {
                TicketId = parsedId,
                User = user,
                Message = message,
                Timestamp = DateTime.Now
            };

            db.ChatMessages.Add(chatMessage);
            await db.SaveChangesAsync();

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