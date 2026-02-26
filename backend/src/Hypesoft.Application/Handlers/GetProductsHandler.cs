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
    private const string CacheKeyPrefix = "products_cache";

    public GetProductsHandler(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PagedResponse<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {

        var (items, totalCount) = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        
        var response = new PagedResponse<ProductResponse>(
            items.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, p.IsStockLow())),
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        return response;
    }
}