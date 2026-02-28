using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hypesoft.Infrastructure.Data;

namespace Hypesoft.IntegrationTests;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"HypesoftIntegrationTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set all required environment variables BEFORE host creation
        // These are read early in the configuration process
        Environment.SetEnvironmentVariable("USE_IN_MEMORY_DB", "true");
        Environment.SetEnvironmentVariable("DISABLE_AUTH", "true");
        Environment.SetEnvironmentVariable("KEYCLOAK_INTERNAL_URL", "http://localhost:8080/realms/hypesoft");
        Environment.SetEnvironmentVariable("KEYCLOAK_EXTERNAL_URL", "http://localhost:8080/realms/hypesoft");
        Environment.SetEnvironmentVariable("KEYCLOAK_AUDIENCE", "account");
        Environment.SetEnvironmentVariable("CORS_ORIGINS", "http://localhost:3000");
        
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add the same settings via configuration as a fallback
            var settings = new Dictionary<string, string?>
            {
                ["USE_IN_MEMORY_DB"] = "true",
                ["ConnectionStrings:MongoDb"] = "mongodb://localhost:27017",
                ["ConnectionStrings:DatabaseName"] = _databaseName,
                ["KEYCLOAK_INTERNAL_URL"] = "http://localhost:8080/realms/hypesoft",
                ["KEYCLOAK_EXTERNAL_URL"] = "http://localhost:8080/realms/hypesoft",
                ["KEYCLOAK_AUDIENCE"] = "account",
                ["CORS_ORIGINS"] = "http://localhost:3000",
                ["DISABLE_AUTH"] = "true"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            // Remove the DbContext if it exists and add fresh one for tests
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            // Replace authentication with test auth
            var authDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(IAuthenticationSchemeProvider));
            if (authDescriptor != null)
                services.Remove(authDescriptor);

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
