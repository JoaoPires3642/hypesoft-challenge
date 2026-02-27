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

    [Fact]
    public async Task Put_updates_product_successfully()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Hardware");
        var productId = await CreateProductAsync("Mouse Gamer", "Mouse RGB", 150.00m, 10, categoryId);

        var updatePayload = new
        {
            id = productId,
            name = "Mouse Gamer Pro",
            description = "Mouse RGB com 16000 DPI",
            price = 199.90m,
            stockQuantity = 15,
            categoryId
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/products/{productId}", updatePayload);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/products/{productId}");
        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be(updatePayload.name);
        updatedProduct.Description.Should().Be(updatePayload.description);
        updatedProduct.Price.Should().Be(updatePayload.price);
        updatedProduct.StockQuantity.Should().Be(updatePayload.stockQuantity);
    }

    [Fact]
    public async Task Put_returns_bad_request_when_id_mismatch()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Hardware");
        var productId = await CreateProductAsync("Mouse", "Desc", 100m, 5, categoryId);
        var differentId = Guid.NewGuid();

        var updatePayload = new
        {
            id = differentId,
            name = "Mouse Atualizado",
            description = "Nova descrição",
            price = 150m,
            stockQuantity = 10,
            categoryId
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/products/{productId}", updatePayload);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_removes_product_successfully()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Perifericos");
        var productId = await CreateProductAsync("Webcam", "Webcam HD", 250.00m, 8, categoryId);

        var deleteResponse = await Client.DeleteAsync($"/api/products/{productId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_returns_not_found_for_nonexistent_product()
    {
        await ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        var deleteResponse = await Client.DeleteAsync($"/api/products/{nonExistentId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Patch_updates_stock_successfully()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Componentes");
        var productId = await CreateProductAsync("SSD 1TB", "SSD NVMe", 500.00m, 20, categoryId);

        var newStockQuantity = 35;
        var updateResponse = await Client.PatchAsync(
            $"/api/products/{productId}/stock",
            JsonContent.Create(newStockQuantity)
        );

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await Client.GetAsync($"/api/products/{productId}");
        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        product.Should().NotBeNull();
        product!.StockQuantity.Should().Be(newStockQuantity);
    }

    [Fact]
    public async Task Patch_returns_not_found_for_nonexistent_product()
    {
        await ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        var updateResponse = await Client.PatchAsync(
            $"/api/products/{nonExistentId}/stock",
            JsonContent.Create(50)
        );

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_returns_matching_products()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Eletronicos");
        await CreateProductAsync("Teclado Mecanico", "Teclado RGB", 300m, 10, categoryId);
        await CreateProductAsync("Mouse Gamer", "Mouse com LED", 150m, 20, categoryId);
        await CreateProductAsync("Monitor 24pol", "Monitor Full HD", 800m, 5, categoryId);

        var searchResponse = await Client.GetAsync("/api/products/search?name=Tecl");

        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await searchResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        products.Should().NotBeNull();
        products!.Should().HaveCount(1);
        products.First().Name.Should().Contain("Teclado");
    }

    [Fact]
    public async Task Search_returns_empty_for_no_matches()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Hardware");
        await CreateProductAsync("Mouse", "Descrição", 100m, 5, categoryId);

        var searchResponse = await Client.GetAsync("/api/products/search?name=Notebook");

        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await searchResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        products.Should().NotBeNull();
        products!.Should().BeEmpty();
    }

    [Fact]
    public async Task Search_returns_bad_request_for_empty_term()
    {
        await ResetDatabaseAsync();

        var searchResponse = await Client.GetAsync("/api/products/search?name=");

        searchResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByCategory_returns_filtered_products()
    {
        await ResetDatabaseAsync();

        var category1Id = await CreateCategoryAsync("Perifericos");
        var category2Id = await CreateCategoryAsync("Componentes");

        var product1Id = await CreateProductAsync("Teclado", "Desc", 200m, 10, category1Id);
        var product2Id = await CreateProductAsync("Mouse", "Desc", 100m, 15, category1Id);
        await CreateProductAsync("SSD", "Desc", 500m, 5, category2Id);

        var filterResponse = await Client.GetAsync($"/api/products/category/{category1Id}");

        filterResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await filterResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        products.Should().NotBeNull();
        products!.Should().HaveCount(2);
        products.Should().Contain(p => p.Id == product1Id);
        products.Should().Contain(p => p.Id == product2Id);
        products.Should().OnlyContain(p => p.CategoryId == category1Id);
    }

    [Fact]
    public async Task GetByCategory_returns_empty_for_nonexistent_category()
    {
        await ResetDatabaseAsync();

        var nonExistentCategoryId = Guid.NewGuid();

        var filterResponse = await Client.GetAsync($"/api/products/category/{nonExistentCategoryId}");

        filterResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await filterResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        products.Should().NotBeNull();
        products!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLowStock_returns_products_below_threshold()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Hardware");
        var lowStockId1 = await CreateProductAsync("Produto 1", "Desc", 100m, 5, categoryId);
        var lowStockId2 = await CreateProductAsync("Produto 2", "Desc", 150m, 8, categoryId);
        await CreateProductAsync("Produto 3", "Desc", 200m, 15, categoryId);

        var lowStockResponse = await Client.GetAsync("/api/products/low-stock");

        lowStockResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await lowStockResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        products.Should().NotBeNull();
        products!.Should().HaveCount(2);
        products.Should().Contain(p => p.Id == lowStockId1);
        products.Should().Contain(p => p.Id == lowStockId2);
        products.Should().OnlyContain(p => p.StockQuantity < 10);
    }

    private async Task<Guid> CreateCategoryAsync(string name)
    {
        var createResponse = await Client.PostAsJsonAsync("/api/categories", new { name });
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var id = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        id.Should().NotBeEmpty();
        return id;
    }

    private async Task<Guid> CreateProductAsync(
        string name,
        string description,
        decimal price,
        int stockQuantity,
        Guid categoryId)
    {
        var payload = new
        {
            name,
            description,
            price,
            stockQuantity,
            categoryId
        };

        var createResponse = await Client.PostAsJsonAsync("/api/products", payload);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
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
