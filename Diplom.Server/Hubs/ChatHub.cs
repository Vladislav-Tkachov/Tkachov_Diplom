using Microsoft.AspNetCore.SignalR;

namespace Diplom.Server.Hubs;

public class ChatHub : Hub
{
    // Метод для надсилання повідомлення у вказану групу
    public async Task SendMessage(string group, string user, string message)
    {
        await Clients.Group(group).SendAsync("ReceiveMessage", user, message);
    }

    // Метод для приєднання користувача до групи (наприклад, пов'язаної з тікетом)
    public async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} приєднався до групи {group}");
    }

    // Метод для покидання групи
    public async Task LeaveGroup(string group)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} покинув групу {group}");
    }
}