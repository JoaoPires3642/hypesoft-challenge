using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Hypesoft.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Hypesoft.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<ApiTestFactory>
{
    protected IntegrationTestBase(ApiTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    protected ApiTestFactory Factory { get; }
    protected HttpClient Client { get; }

    protected JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}
