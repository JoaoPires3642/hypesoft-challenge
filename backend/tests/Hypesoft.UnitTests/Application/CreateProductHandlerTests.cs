using Moq;
using FluentAssertions;
using Hypesoft.Application.Commands;
using Hypesoft.Application.Handlers;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.UnitTests.Application;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ICacheInvalidator> _cacheInvalidatorMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _cacheInvalidatorMock = new Mock<ICacheInvalidator>();
        _handler = new CreateProductHandler(_repositoryMock.Object, _cacheInvalidatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand("Teclado", "Mecânico", 299, 15, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _cacheInvalidatorMock.Verify(
            c => c.InvalidateProductCache(null, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _cacheInvalidatorMock.Verify(
            c => c.InvalidateCategoryProductsCache(command.CategoryId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}