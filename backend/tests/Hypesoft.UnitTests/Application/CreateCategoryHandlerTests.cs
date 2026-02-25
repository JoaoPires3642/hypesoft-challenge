using FluentAssertions;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Moq;

namespace Hypesoft.UnitTests.Application;

public class CreateCategoryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var handler = new CreateCategoryHandler(repositoryMock.Object);

        var result = await handler.Handle(new CreateCategoryCommand("Eletrônicos"), CancellationToken.None);

        result.Should().NotBeEmpty();
        repositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "Eletrônicos")), Times.Once);
    }
}
