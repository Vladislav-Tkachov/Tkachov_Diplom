using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Diplom;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Настройка HttpClient для API сервера
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });

//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Diplom"));

// Добавляем API авторизацию
builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();