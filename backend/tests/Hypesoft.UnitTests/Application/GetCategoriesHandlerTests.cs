using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class GetCategoriesHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var categories = new List<Category>
        {
            new("Informática"),
            new("Acessórios")
        };

        repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var handler = new GetCategoriesHandler(repositoryMock.Object);

        var result = (await handler.Handle(new GetCategoriesQuery(), CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().Contain(["Informática", "Acessórios"]);
    }
}
