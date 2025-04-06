using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Diplom.Server.Data;
using Diplom.Server.Hubs;
using Diplom.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Налаштування бази даних (для демонстрації використовується InMemory; для продакшена використовуйте SQL Server чи інше)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TicketsDB"));

// Налаштування Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Налаштування CORS для клієнтського додатку (замість "https://localhost:5002" вкажіть адресу вашого клієнта)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        builder => builder
            .WithOrigins("https://localhost:5002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Реєстрація сервісу матчінгу
builder.Services.AddScoped<MatchingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("index.html");

app.Run();