using FluentAssertions;
using Hypesoft.Domain.Entities;
using Xunit;

namespace Hypesoft.UnitTests.Domain;

public class CategoryTests
{
    [Fact]
    public void Constructor_ShouldCreateCategory_WhenNameIsValid()
    {
        // Act
        var category = new Category("Periféricos");

        // Assert
        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be("Periféricos");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        // Act
        var act = () => new Category("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O nome da Categoria é obrigatório.*");
    }

    [Fact]
    public void UpdateName_ShouldUpdateName_WhenNameIsValid()
    {
        // Arrange
        var category = new Category("Periféricos");

        // Act
        category.UpdateName("Hardware");

        // Assert
        category.Name.Should().Be("Hardware");
    }

    [Fact]
    public void UpdateName_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var category = new Category("Periféricos");

        // Act
        var act = () => category.UpdateName("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O nome da Categoria é obrigatório.*");
    }
}
