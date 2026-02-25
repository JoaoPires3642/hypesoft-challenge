using Hypesoft.Application.DTOs;
using Hypesoft.Application.Queries;
using Hypesoft.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "products_all_";

    public GetProductsHandler(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PagedResponse<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        string currentCacheKey = $"{CacheKey}{request.PageNumber}_{request.PageSize}";
        
        // 1. Tenta obter do Cache
        var cachedData = await _cache.GetStringAsync(currentCacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<PagedResponse<ProductResponse>>(cachedData)!;
        }

        // 2. Se tiver vai ao Banco
        var (items, totalCount) = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        
        var response = new PagedResponse<ProductResponse>(
            items.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, p.IsStockLow())),
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        // 3. Salva no Cache por 10 minutos
        var cacheOptions = new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(currentCacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);

        return response;
    }
}