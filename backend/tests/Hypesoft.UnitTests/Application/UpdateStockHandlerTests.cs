using FluentAssertions;
using Hypesoft.Application.Commands;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class UpdateStockHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateStockHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new UpdateStockCommand(Guid.NewGuid(), 7), CancellationToken.None);

        result.Should().BeFalse();
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStockAndInvalidateCache_WhenProductExists()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var product = new Product("Produto", "Desc", 10m, 2, Guid.NewGuid());

        repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateStockHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new UpdateStockCommand(product.Id, 15), CancellationToken.None);

        result.Should().BeTrue();
        product.StockQuantity.Should().Be(15);
        repositoryMock.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.RemoveAsync("products_all_", It.IsAny<CancellationToken>()), Times.Once);
    }
}
