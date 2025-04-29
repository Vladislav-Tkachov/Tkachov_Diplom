using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Diplom.Shared.Models;

namespace Diplom.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ApiService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task SetAuthorizationHeader()
    {
        var token = await _localStorage.GetItemAsStringAsync("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetTicketsAsync()
    {
        await SetAuthorizationHeader();
        return await _httpClient.GetAsync("api/tickets");
    }

    public async Task<HttpResponseMessage> PostTicketAsync(Ticket ticket)
    {
        await SetAuthorizationHeader();
        return await _httpClient.PostAsJsonAsync("api/tickets", ticket);
    }

    public async Task<HttpResponseMessage> DeleteTicketAsync(int ticketId)
    {
        await SetAuthorizationHeader();
        return await _httpClient.DeleteAsync($"api/tickets/{ticketId}");
    }
    
    public async Task<List<ChatMessage>?> GetChatHistoryAsync(string ticketId)
    {
        await SetAuthorizationHeader();
        return await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"api/chat/{ticketId}");
    }
}