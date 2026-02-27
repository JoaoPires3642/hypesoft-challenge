using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Hypesoft.IntegrationTests;

public class ProductsEndpointsTests : IntegrationTestBase
{
    public ProductsEndpointsTests(ApiTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Post_creates_product_and_get_returns_it()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Perifericos");
        var payload = new
        {
            name = "Teclado Mecanico",
            description = "Teclado com switches azuis",
            price = 399.90m,
            stockQuantity = 5,
            categoryId
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", payload);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var productId = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        productId.Should().NotBeEmpty();

        var getResponse = await Client.GetAsync($"/api/products/{productId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);
        product.Should().NotBeNull();
        product!.Id.Should().Be(productId);
        product.Name.Should().Be(payload.name);
        product.StockQuantity.Should().Be(payload.stockQuantity);
        product.CategoryId.Should().Be(categoryId);

        var listResponse = await Client.GetAsync("/api/products");

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await listResponse.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>(JsonOptions);
        paged.Should().NotBeNull();
        paged!.TotalCount.Should().Be(1);
        paged.Items.Should().ContainSingle(p => p.Id == productId);
    }

    private async Task<Guid> CreateCategoryAsync(string name)
    {
        var createResponse = await Client.PostAsJsonAsync("/api/categories", new { name });
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var id = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        id.Should().NotBeEmpty();
        return id;
    }

    private record ProductDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId,
        bool IsStockLow
    );

    private record PagedResponse<T>(
        List<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount
    );
}
