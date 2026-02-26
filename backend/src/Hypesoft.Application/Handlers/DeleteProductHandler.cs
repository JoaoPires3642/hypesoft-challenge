using Hypesoft.Application.Commands;
using Hypesoft.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Hypesoft.Application.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;

    public DeleteProductHandler(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;

        await _repository.DeleteAsync(request.Id, cancellationToken);
        await _cache.RemoveAsync("products_cache", cancellationToken);

        Log.Warning("Produto {ProductId} removido do sistema", request.Id);
        return true;
    }
}
