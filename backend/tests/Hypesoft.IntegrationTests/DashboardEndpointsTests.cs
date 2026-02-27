using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Hypesoft.IntegrationTests;

public class DashboardEndpointsTests : IntegrationTestBase
{
    public DashboardEndpointsTests(ApiTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetSummary_returns_dashboard_data()
    {
        await ResetDatabaseAsync();

        var categoryId1 = await CreateCategoryAsync("Hardware");
        var categoryId2 = await CreateCategoryAsync("Perifericos");
        
        await CreateProductAsync("Mouse", "Mouse gamer", 150m, 25, categoryId1);
        await CreateProductAsync("Teclado", "Teclado mecânico", 300m, 15, categoryId1);
        await CreateProductAsync("Webcam", "Webcam HD", 200m, 5, categoryId2);
        await CreateProductAsync("Headset", "Headset 7.1", 400m, 8, categoryId2);

        var response = await Client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>(JsonOptions);

        dashboard.Should().NotBeNull();
        dashboard!.TotalProducts.Should().Be(4);
        dashboard.ProductsByCategory.Should().HaveCount(2);
        dashboard.LowStockProducts.Should().HaveCount(2);
        dashboard.TotalStockValue.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSummary_returns_zero_values_for_empty_database()
    {
        await ResetDatabaseAsync();

        var response = await Client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>(JsonOptions);

        dashboard.Should().NotBeNull();
        dashboard!.TotalProducts.Should().Be(0);
        dashboard.ProductsByCategory.Should().BeEmpty();
        dashboard.LowStockProducts.Should().BeEmpty();
        dashboard.TotalStockValue.Should().Be(0);
    }

    [Fact]
    public async Task GetSummary_calculates_inventory_value_correctly()
    {
        await ResetDatabaseAsync();

        var categoryId = await CreateCategoryAsync("Testes");
        
        await CreateProductAsync("Produto 1", "Desc", 100m, 10, categoryId);
        await CreateProductAsync("Produto 2", "Desc", 200m, 5, categoryId);

        var response = await Client.GetAsync("/api/dashboard");

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>(JsonOptions);

        dashboard.Should().NotBeNull();
        dashboard!.TotalStockValue.Should().Be(2000m);
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

    private record DashboardDto(
        int TotalProducts,
        decimal TotalStockValue,
        List<ProductDto> LowStockProducts,
        List<CategoryChartDataDto> ProductsByCategory
    );

    private record ProductDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId,
        bool IsStockLow
    );

    private record CategoryChartDataDto(string CategoryName, int ProductCount);
}
