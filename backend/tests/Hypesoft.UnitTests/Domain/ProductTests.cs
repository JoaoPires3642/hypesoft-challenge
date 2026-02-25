using FluentAssertions;
using Hypesoft.Domain.Entities;
using Xunit;

namespace Hypesoft.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateProduct_WhenDataIsValid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var product = new Product("SSD 1TB", "High speed", 500, 5, categoryId);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("SSD 1TB");
        product.Description.Should().Be("High speed");
        product.Price.Should().Be(500);
        product.StockQuantity.Should().Be(5);
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        // Act
        var act = () => new Product("", "High speed", 500, 5, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O nome do Produto é obrigatório.*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsNegative()
    {
        // Act
        var act = () => new Product("SSD 1TB", "High speed", -1, 5, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O preço do Produto não pode ser negativo.*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenStockIsNegative()
    {
        // Act
        var act = () => new Product("SSD 1TB", "High speed", 500, -1, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("A quantidade em estoque do Produto não pode ser negativa.*");
    }

    [Fact]
    public void IsStockLow_ShouldReturnTrue_WhenStockIsLessThan10()
    {
        // Arrange
        var product = new Product("SSD 1TB", "High speed", 500, 5, Guid.NewGuid());

        // Act
        var result = product.IsStockLow();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsStockLow_ShouldReturnFalse_WhenStockIsGreaterOrEqualTo10()
    {
        // Arrange
        var product = new Product("SSD 1TB", "High speed", 500, 10, Guid.NewGuid());

        // Act
        var result = product.IsStockLow();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateStock_ShouldThrowException_WhenValueIsNegative()
    {
        // Arrange
        var product = new Product("Mouse", "RGB", 50, 20, Guid.NewGuid());

        // Act
        var act = () => product.UpdateStock(-1);

        // Assert
        act.Should().Throw<ArgumentException>()
              .WithMessage("A quantidade em estoque do Produto não pode ser negativa.*");
    }

    [Fact]
    public void UpdateStock_ShouldUpdateStock_WhenValueIsValid()
    {
        // Arrange
        var product = new Product("Mouse", "RGB", 50, 20, Guid.NewGuid());

        // Act
        product.UpdateStock(12);

        // Assert
        product.StockQuantity.Should().Be(12);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateFields_WhenDataIsValid()
    {
        // Arrange
        var product = new Product("Mouse", "RGB", 50, 20, Guid.NewGuid());

        // Act
        product.UpdateDetails("Mouse Pro", "RGB Wireless", 99);

        // Assert
        product.Name.Should().Be("Mouse Pro");
        product.Description.Should().Be("RGB Wireless");
        product.Price.Should().Be(99);
    }

    [Fact]
    public void UpdateDetails_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var product = new Product("Mouse", "RGB", 50, 20, Guid.NewGuid());

        // Act
        var act = () => product.UpdateDetails("", "RGB Wireless", 99);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O nome do Produto é obrigatório.*");
    }

    [Fact]
    public void UpdateDetails_ShouldThrowException_WhenPriceIsNegative()
    {
        // Arrange
        var product = new Product("Mouse", "RGB", 50, 20, Guid.NewGuid());

        // Act
        var act = () => product.UpdateDetails("Mouse Pro", "RGB Wireless", -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("O preço do Produto não pode ser negativo.*");
    }
}