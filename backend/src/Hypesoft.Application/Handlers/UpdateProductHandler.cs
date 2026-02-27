using Hypesoft.Application.Commands;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Repositories;
using MediatR;
using Serilog;

namespace Hypesoft.Application.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ICacheInvalidator _cacheInvalidator;

    public UpdateProductHandler(IProductRepository repository, ICacheInvalidator cacheInvalidator)
    {
        _repository = repository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;

        product.UpdateDetails(request.Name, request.Description, request.Price);
        product.UpdateStock(request.StockQuantity);

        await _repository.UpdateAsync(product, cancellationToken);
        await _cacheInvalidator.InvalidateProductCache(product.Id, cancellationToken);
        await _cacheInvalidator.InvalidateCategoryProductsCache(product.CategoryId, cancellationToken);
        await _cacheInvalidator.InvalidateProductCache(null, cancellationToken);

        Log.Information("Produto {ProductId} atualizado com sucesso", product.Id);
        return true;
    }
}

