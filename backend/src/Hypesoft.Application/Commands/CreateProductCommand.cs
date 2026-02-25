using MediatR;

namespace Hypesoft.Application.Commands;

/// <summary>
/// Comando para criar um novo produto no sistema
/// </summary>
/// <param name="Name">Nome do produto (obrigatório, máximo 255 caracteres)</param>
/// <param name="Description">Descrição detalhada do produto (obrigatório)</param>
/// <param name="Price">Preço do produto em reais (obrigatório, maior que 0)</param>
/// <param name="StockQuantity">Quantidade em estoque (obrigatório, maior ou igual a 0)</param>
/// <param name="CategoryId">ID da categoria do produto (obrigatório)</param>
/// <example>
/// <code>
/// var command = new CreateProductCommand(
///     Name: "Notebook Dell XPS 13",
///     Description: "Notebook ultraportátil com processador Intel i7 e SSD 512GB",
///     Price: 4500.00m,
///     StockQuantity: 10,
///     CategoryId: new Guid("550e8400-e29b-41d4-a716-446655440000")
/// );
/// </code>
/// </example>
public record CreateProductCommand(
    string Name, 
    string Description, 
    decimal Price, 
    int StockQuantity, 
    Guid CategoryId
) : IRequest<Guid>; 