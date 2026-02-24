using MediatR;

namespace Hypesoft.Application.Commands;


public record CreateProductCommand(
    string Name, 
    string Description, 
    decimal Price, 
    int StockQuantity, 
    Guid CategoryId
) : IRequest<Guid>; 