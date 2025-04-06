using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Diplom.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string group, string user, string message)
        {
            if (!Context.User.Identity?.IsAuthenticated ?? true)
            {
                throw new HubException("Користувач не авторизований");
            }

            await Clients.Group(group).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string group)
        {
            if (!Context.User.Identity?.IsAuthenticated ?? true)
            {
                throw new HubException("Доступ заборонено. Увійдіть у систему.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task LeaveGroup(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}