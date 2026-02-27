using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hypesoft.Infrastructure.Data;

namespace Hypesoft.IntegrationTests;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"HypesoftIntegrationTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("USE_IN_MEMORY_DB", "true");
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["USE_IN_MEMORY_DB"] = "true",
                ["ConnectionStrings:MongoDb"] = "mongodb://localhost:27017",
                ["ConnectionStrings:DatabaseName"] = _databaseName
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            // Remove o DbContext existente e adiciona um novo com nome de banco único
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test",
                    _ => { }
                );
        });
    }
}
