using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Mappings;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class SearchProductsByNameHandlerTests
{
    [Fact]
    public async Task Handle_ShouldMapProductsToResponse()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var mapper = CreateMapper();
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new("Mouse Logitech", "Sem fio", 100m, 5, categoryId)
        };

        repositoryMock
            .Setup(r => r.SearchAsync("Mouse", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new SearchProductsByNameHandler(repositoryMock.Object, mapper);

        var result = (await handler.Handle(new SearchProductsByNameQuery("Mouse"), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Mouse Logitech");
        result[0].IsStockLow.Should().BeTrue();
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
