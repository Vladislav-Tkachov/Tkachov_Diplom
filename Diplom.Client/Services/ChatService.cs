using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace Diplom.Client.Services;

public class ChatService : IAsyncDisposable
{
    private HubConnection _hubConnection;
    private readonly ILocalStorageService _localStorage;

    public event Action<string, string> OnMessageReceived;

    public ChatService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task StartConnectionAsync(string baseUrl)
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            return;

        var token = await _localStorage.GetItemAsync<string>("authToken");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(baseUrl + "chathub",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("ReceiveMessage",
            (user, message) => { OnMessageReceived?.Invoke(user, message); });

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

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}