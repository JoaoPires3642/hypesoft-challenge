using MediatR;
using Hypesoft.Domain.Repositories;
using Serilog;

namespace Hypesoft.Application.Commands;

public record UpdateStockCommand(Guid ProductId, int NewQuantity) : IRequest<bool>;

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand, bool>
{
    private readonly IProductRepository _repository;
    public UpdateStockHandler(IProductRepository repository) => _repository = repository;

    public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId);
        if (product == null) return false;

        
        product.UpdateStock(request.NewQuantity);

        await _repository.UpdateAsync(product);
        
        Log.Information("Estoque do produto {ProductId} atualizado para {Quantity}", 
            product.Id, request.NewQuantity);
            
        return true;
    }
}