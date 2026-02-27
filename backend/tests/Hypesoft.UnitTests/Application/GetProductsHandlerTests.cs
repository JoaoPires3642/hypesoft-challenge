using FluentAssertions;
using Hypesoft.Application.DTOs;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace Hypesoft.UnitTests.Application;

public class GetProductsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnProductsFromRepository()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var categoryId = Guid.NewGuid();
        var product = new Product("Prod", "Desc", 1m, 1, categoryId);

        repositoryMock
            .Setup(r => r.GetAllPagedAsync(1, 10))
            .ReturnsAsync(([product], 1));

        var handler = new GetProductsHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        repositoryMock.Verify(r => r.GetAllPagedAsync(1, 10), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleProducts()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var categoryId = Guid.NewGuid();
        var items = new List<Product>
        {
            new("Mouse", "Sem fio", 120m, 8, categoryId),
            new("Teclado", "Mecânico", 300m, 20, categoryId)
        };

        repositoryMock
            .Setup(r => r.GetAllPagedAsync(1, 10))
            .ReturnsAsync((items.AsEnumerable(), items.Count));

        var handler = new GetProductsHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        repositoryMock.Verify(r => r.GetAllPagedAsync(1, 10), Times.Once);
    }
}
