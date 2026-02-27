using AutoMapper;
using Hypesoft.Application.DTOs;
using Hypesoft.Application.Queries;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly IMapper _mapper;
    private static readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public GetProductsHandler(IProductRepository repository, IDistributedCache cache, IMapper mapper)
    {
        _repository = repository;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductsPaged(request.PageNumber, request.PageSize);
        
        // Tenta buscar do cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<PagedResponse<ProductResponse>>(cachedData);
        }

        // Se não estiver no cache, busca do repositório
        var (items, totalCount) = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        
        var response = new PagedResponse<ProductResponse>(
            _mapper.Map<IEnumerable<ProductResponse>>(items),
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        // Armazena no cache
        var jsonData = JsonSerializer.Serialize(response);
        await _cache.SetStringAsync(cacheKey, jsonData, _cacheOptions, cancellationToken);

        return response;
    }
}