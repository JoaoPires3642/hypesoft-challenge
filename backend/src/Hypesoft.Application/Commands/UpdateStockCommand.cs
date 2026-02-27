using MediatR;
using Hypesoft.Domain.Repositories;
using Hypesoft.Application.Infrastructure.Cache;
using Serilog;

namespace Hypesoft.Application.Commands;

public record UpdateStockCommand(Guid ProductId, int NewQuantity) : IRequest<bool>;

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ICacheInvalidator _cacheInvalidator;
    
    public UpdateStockHandler(IProductRepository repository, ICacheInvalidator cacheInvalidator)
    {
        _repository = repository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return false;

        product.UpdateStock(request.NewQuantity);

        await _repository.UpdateAsync(product, cancellationToken);
    
        await _cacheInvalidator.InvalidateProductCache(product.Id, cancellationToken);
        
        Log.Information("Estoque do produto {ProductId} atualizado para {Quantity}", 
            product.Id, request.NewQuantity);
            
        return true;
    }
}