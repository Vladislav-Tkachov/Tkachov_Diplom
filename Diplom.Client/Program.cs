using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Diplom.Client.Client;
using System.Net.Http;
using Diplom;
using Diplom.Client;
using Diplom.Client.Services;
using Diplom.Client.Shared;
using Diplom.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ChatService>();

await builder.Build().RunAsync();