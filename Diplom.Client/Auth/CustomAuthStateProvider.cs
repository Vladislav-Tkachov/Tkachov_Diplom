using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace Diplom.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private HttpClient _httpClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, NavigationManager navigationManager, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _httpClient = httpClient;
        }
        
        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var loginModel = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var token = await response.Content.ReadAsStringAsync();

                await _localStorage.SetItemAsync("authToken", token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            catch
            {
                return false;
            }
        }



        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _navigationManager.NavigateTo("/login");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigationManager.NavigateTo("/login");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigationManager.NavigateTo("/login");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task NotifyUserAuthentication(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
