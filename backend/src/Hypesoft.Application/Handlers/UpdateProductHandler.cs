using Hypesoft.Application.Commands;
using Hypesoft.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Hypesoft.Application.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;

    public UpdateProductHandler(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;

        product.UpdateDetails(request.Name, request.Description, request.Price);
        product.UpdateStock(request.StockQuantity);

        await _repository.UpdateAsync(product, cancellationToken);
        await _cache.RemoveAsync("products_all_");

        Log.Information("Produto {ProductId} atualizado com sucesso", product.Id);
        return true;
    }
}
