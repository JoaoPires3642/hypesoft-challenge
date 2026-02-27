using Hypesoft.Application.Commands;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Repositories;
using MediatR;
using Serilog;

namespace Hypesoft.Application.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ICacheInvalidator _cacheInvalidator;

    public DeleteProductHandler(IProductRepository repository, ICacheInvalidator cacheInvalidator)
    {
        _repository = repository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;

        await _repository.DeleteAsync(request.Id, cancellationToken);
        
        // Invalidação granular: remove apenas este produto específico
        await _cacheInvalidator.InvalidateProductCache(product.Id, cancellationToken);
        // Invalida também a categoria
        await _cacheInvalidator.InvalidateCategoryProductsCache(product.CategoryId, cancellationToken);
        // Invalida as listagens paginadas
        await _cacheInvalidator.InvalidateProductCache(null, cancellationToken);

        Log.Warning("Produto {ProductId} removido do sistema", request.Id);
        return true;
    }
}
