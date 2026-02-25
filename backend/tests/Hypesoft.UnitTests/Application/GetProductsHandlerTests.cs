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
    public async Task Handle_ShouldReturnFromCache_WhenCacheHasData()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var cached = new PagedResponse<ProductResponse>(
            [new ProductResponse(Guid.NewGuid(), "Prod", "Desc", 1m, 1, Guid.NewGuid(), true)],
            1,
            10,
            1);

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cached));
        cacheMock
            .Setup(c => c.GetAsync("products_all_1_10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var handler = new GetProductsHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        repositoryMock.Verify(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReadRepositoryAndCache_WhenCacheMiss()
    {
        var repositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<IDistributedCache>();
        var categoryId = Guid.NewGuid();
        var items = new List<Product>
        {
            new("Mouse", "Sem fio", 120m, 8, categoryId),
            new("Teclado", "Mecânico", 300m, 20, categoryId)
        };

        cacheMock
            .Setup(c => c.GetAsync("products_all_1_10", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        repositoryMock
            .Setup(r => r.GetAllPagedAsync(1, 10))
            .ReturnsAsync((items.AsEnumerable(), items.Count));

        var handler = new GetProductsHandler(repositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        cacheMock.Verify(c => c.SetAsync(
            "products_all_1_10",
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
