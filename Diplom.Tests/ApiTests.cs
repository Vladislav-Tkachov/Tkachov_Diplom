using System.Net;
using FluentAssertions;
using Xunit;

namespace Diplom.Tests;

public class ApiTests : IClassFixture<TestWebHostFactory>
{
    private readonly HttpClient _client;

    public ApiTests(TestWebHostFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Get_Tickets_Returns200()
    {
        var response = await _client.GetAsync("/api/tickets"); // контролер TicketsController :contentReference[oaicite:3]{index=3}
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}