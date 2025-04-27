using Microsoft.AspNetCore.SignalR.Client;

namespace Diplom.Client.Services;

public class ChatService
{
    private HubConnection _hubConnection;

    public event Action<string, string> OnMessageReceived;

    public async Task StartConnectionAsync(string baseUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(baseUrl + "chathub")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            OnMessageReceived?.Invoke(user, message);
        });

        await _hubConnection.StartAsync();
    }

    public async Task JoinTicketRoomAsync(string ticketId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("JoinTicketRoom", ticketId);
        }
    }

    public async Task LeaveTicketRoomAsync(string ticketId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("LeaveTicketRoom", ticketId);
        }
    }

    public async Task SendMessageAsync(string ticketId, string user, string message)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SendMessage", ticketId, user, message);
        }
    }
}