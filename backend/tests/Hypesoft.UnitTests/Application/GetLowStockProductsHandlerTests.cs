using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Mappings;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Constants;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class GetLowStockProductsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRequestThreshold10_AndMapResponse()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var mapper = CreateMapper();
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new("Cabo", "USB-C", 30m, 2, categoryId)
        };

        repositoryMock
            .Setup(r => r.GetLowStockAsync(ProductConstants.LOW_STOCK_THRESHOLD, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetLowStockProductsHandler(repositoryMock.Object, mapper);

        var result = (await handler.Handle(new GetLowStockProductsQuery(), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].IsStockLow.Should().BeTrue();
        repositoryMock.Verify(r => r.GetLowStockAsync(ProductConstants.LOW_STOCK_THRESHOLD, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        });
        return config.CreateMapper();
    }
}
