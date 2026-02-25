using Moq;
using FluentAssertions;
using Hypesoft.Application.Commands;
using Hypesoft.Application.Handlers;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Hypesoft.UnitTests.Application;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _cacheMock = new Mock<IDistributedCache>();
        _handler = new CreateProductHandler(_repositoryMock.Object, _cacheMock.Object);
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
        _cacheMock.Verify(c => c.RemoveAsync("products_all_", It.IsAny<CancellationToken>()), Times.Once);
    }
}