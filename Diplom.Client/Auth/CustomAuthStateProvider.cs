using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Diplom.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }


        public async Task<bool> Login(string email, string password)
        {
            var loginModel = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var token = await response.Content.ReadAsStringAsync();
            await _localStorage.SetItemAsync("authToken", token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
