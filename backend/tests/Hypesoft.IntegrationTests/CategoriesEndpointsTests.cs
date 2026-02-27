using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Hypesoft.IntegrationTests;

public class CategoriesEndpointsTests : IntegrationTestBase
{
    public CategoriesEndpointsTests(ApiTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Post_creates_category_and_get_returns_it()
    {
        await ResetDatabaseAsync();

        var payload = new { name = "Eletronicos" };

        var createResponse = await Client.PostAsJsonAsync("/api/categories", payload);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        createdId.Should().NotBeEmpty();

        var listResponse = await Client.GetAsync("/api/categories");

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await listResponse.Content.ReadFromJsonAsync<List<CategoryDto>>(JsonOptions);
        categories.Should().NotBeNull();
        categories!.Should().ContainSingle(c => c.Id == createdId && c.Name == payload.name);
    }

    private record CategoryDto(Guid Id, string Name);
}
