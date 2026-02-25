using FluentAssertions;
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
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new("Notebook", "i7", 5000m, 3, categoryId)
        };

        repositoryMock
            .Setup(r => r.SearchAsync("note", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new SearchProductsByNameHandler(repositoryMock.Object);

        var result = (await handler.Handle(new SearchProductsByNameQuery("note"), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Notebook");
        result[0].IsStockLow.Should().BeTrue();
    }
}
