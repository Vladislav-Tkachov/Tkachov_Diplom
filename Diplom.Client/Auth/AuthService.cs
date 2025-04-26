using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diplom.Shared.Models;
using Blazored.LocalStorage;

namespace Diplom.Client
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, CustomAuthStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<bool> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
                return false;

            var token = await response.Content.ReadAsStringAsync();
            await _authStateProvider.MarkUserAsAuthenticated(token);

            return true;
        }

        public async Task<bool> Register(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _authStateProvider.MarkUserAsLoggedOut();
        }
    }
}