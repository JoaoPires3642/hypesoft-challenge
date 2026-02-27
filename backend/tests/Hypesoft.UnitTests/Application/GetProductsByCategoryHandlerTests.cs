using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Mappings;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class GetProductsByCategoryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyMappedProducts()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var mapper = CreateMapper();
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new("Produto", "Desc", 100m, 12, categoryId)
        };

        repositoryMock
            .Setup(r => r.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetProductsByCategoryHandler(repositoryMock.Object, mapper);

        var result = (await handler.Handle(new GetProductsByCategoryQuery(categoryId), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].CategoryId.Should().Be(categoryId);
        result[0].IsStockLow.Should().BeFalse();
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
