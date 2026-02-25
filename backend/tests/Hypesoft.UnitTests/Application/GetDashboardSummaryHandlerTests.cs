using FluentAssertions;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class GetDashboardSummaryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAggregateDashboardData()
    {
        var productRepoMock = new Mock<IProductRepository>();
        var categoryRepoMock = new Mock<ICategoryRepository>();

        var cat1 = new Category("Periféricos");
        var cat2 = new Category("Monitores");
        var lowStockProducts = new List<Product>
        {
            new("Mouse", "Sem fio", 100m, 2, cat1.Id)
        };

        productRepoMock.Setup(r => r.GetTotalCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(12);
        productRepoMock.Setup(r => r.GetTotalStockValueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(25000m);
        productRepoMock.Setup(r => r.GetLowStockAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(lowStockProducts);
        productRepoMock.Setup(r => r.GetCountByCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int> { [cat1.Id] = 5 });
        categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category> { cat1, cat2 });

        var handler = new GetDashboardSummaryHandler(productRepoMock.Object, categoryRepoMock.Object);

        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.TotalProducts.Should().Be(12);
        result.TotalStockValue.Should().Be(25000m);
        result.LowStockProducts.Should().HaveCount(1);
        result.ProductsByCategory.Should().Contain(c => c.CategoryName == "Periféricos" && c.ProductCount == 5);
        result.ProductsByCategory.Should().Contain(c => c.CategoryName == "Monitores" && c.ProductCount == 0);
    }
}
