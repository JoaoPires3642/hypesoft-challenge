using MediatR;
using Hypesoft.Domain.Repositories;
using Serilog;
using Microsoft.Extensions.Caching.Distributed;

namespace Hypesoft.Application.Commands;

public record UpdateStockCommand(Guid ProductId, int NewQuantity) : IRequest<bool>;

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;
    public UpdateStockHandler(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId);
        if (product == null) return false;

        
        product.UpdateStock(request.NewQuantity);

        await _repository.UpdateAsync(product);
        await _cache.RemoveAsync("products_all_");
        
        Log.Information("Estoque do produto {ProductId} atualizado para {Quantity}", 
            product.Id, request.NewQuantity);
            
        return true;
    }
}