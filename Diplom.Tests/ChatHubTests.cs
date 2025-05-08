using Diplom.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Diplom.Tests;

using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

public class ChatHubTests : IAsyncLifetime
{
    private readonly TestWebHostFactory _factory = new();
    private HubConnection _hub = default!;

    public async Task InitializeAsync()
    {
        _hub = new HubConnectionBuilder()
            .WithUrl("http://localhost/chathub", options =>
            {
                // Важливо спрямувати HTTP у in-memory TestServer
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                options.Transports = HttpTransportType.LongPolling;   // WebSockets недоступний у TestServer
            })
            .Build();

        await _hub.StartAsync();
    }

    public async Task DisposeAsync() => await _hub.DisposeAsync();

    [Fact]
    public async Task SendMessage_Broadcasts_User_And_Message()
    {
        string? receivedUser = null;
        string? receivedText = null;

        _hub.On<string, string>("ReceiveMessage", (u, m) =>
        {
            receivedUser = u;
            receivedText = m;
        });

        await _hub.InvokeAsync("JoinTicketRoom", "1");
        await _hub.InvokeAsync("SendMessage", "1", "UnitTester", "hello"); // сигнатура з ChatHub.cs :contentReference[oaicite:2]{index=2}
        await Task.Delay(100);

        receivedUser.Should().Be("UnitTester");
        receivedText.Should().Be("hello");
    }
}