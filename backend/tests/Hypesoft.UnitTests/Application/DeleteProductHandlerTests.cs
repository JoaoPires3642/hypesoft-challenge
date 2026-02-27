using FluentAssertions;
using Hypesoft.Application.Commands;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class DeleteProductHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheInvalidator>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new Hypesoft.Application.Handlers.DeleteProductHandler(repositoryMock.Object, cacheMock.Object);
        var command = new DeleteProductCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        cacheMock.Verify(c => c.InvalidateProductCache(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
        cacheMock.Verify(c => c.InvalidateCategoryProductsCache(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteAndInvalidateCache_WhenProductExists()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheInvalidator>();
        var product = new Product("Produto", "Desc", 10m, 1, Guid.NewGuid());

        repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new Hypesoft.Application.Handlers.DeleteProductHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        result.Should().BeTrue();
        repositoryMock.Verify(r => r.DeleteAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.InvalidateProductCache(product.Id, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.InvalidateCategoryProductsCache(product.CategoryId, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.InvalidateProductCache(null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
