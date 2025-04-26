using Blazored.LocalStorage;
using Diplom;
using Diplom.Client.Auth;
using Diplom.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Реєстрація LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Реєстрація сервісу авторизації
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

// HttpClient з токеном із LocalStorage
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();

    return new HttpClient
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
});


// Toast для повідомлень
builder.Services.AddScoped<ToastService>();

await builder.Build().RunAsync();