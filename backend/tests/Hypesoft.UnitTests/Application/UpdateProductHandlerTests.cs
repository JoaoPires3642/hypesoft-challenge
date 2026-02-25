using FluentAssertions;
using Hypesoft.Application.Commands;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using UpdateProductHandler = Hypesoft.Application.Handlers.UpdateProductHandler;

namespace Hypesoft.UnitTests.Application;

public class UpdateProductHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductHandler(repositoryMock.Object, cacheMock.Object);
        var command = new UpdateProductCommand(Guid.NewGuid(), "Novo", "Desc", 10m, 5);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAndInvalidateCache_WhenProductExists()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var product = new Product("Antigo", "Desc", 5m, 2, Guid.NewGuid());

        repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductHandler(repositoryMock.Object, cacheMock.Object);
        var command = new UpdateProductCommand(product.Id, "Novo", "Nova Desc", 20m, 8);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        repositoryMock.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.RemoveAsync("products_all_", It.IsAny<CancellationToken>()), Times.Once);
        product.Name.Should().Be("Novo");
        product.StockQuantity.Should().Be(8);
    }
}
