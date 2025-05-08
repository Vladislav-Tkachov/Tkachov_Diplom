using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Diplom.Client.Server.Data;
using Diplom.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;   
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diplom.Tests;

public sealed class TestWebHostFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development").ConfigureLogging(l => l.AddConsole());
        

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            
            services.Configure<JsonOptions>(o =>
            {
                o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            
            services.AddDbContext<ApplicationDbContext>(o =>
                o.UseInMemoryDatabase("TicketsTestDb"));
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (!db.Tickets.Any())
            {
                db.Tickets.Add(new Ticket
                {
                    Id = 1, Title = "Test Ticket", Description = "Test Ticket", Category = "Test", Location = "Kh",
                    Type = "Test"
                });
                db.SaveChanges();
            }
            
            // ➊   Додаємо власну схему "Test" із підробним handler'ом
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", _ => { });

            // ➋   Після реєстрації схеми перепризначаємо "за замовчуванням"
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme    = "Test";
                options.DefaultScheme            = "Test";
            });
        });
    }
}

/// <summary>
///  Підробна аутентифікація: пропускає будь-кого як користувача "UnitTester".
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> o,
        ILoggerFactory l, UrlEncoder e, ISystemClock c)
        : base(o, l, e, c) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "UnitTester")
        }, Scheme.Name);

        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}