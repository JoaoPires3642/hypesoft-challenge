using Microsoft.Extensions.Caching.Distributed;

namespace Hypesoft.Application.Infrastructure.Cache;

/// <summary>
/// Serviço para gerenciar invalidação granular de cache.
/// </summary>
public interface ICacheInvalidator
{
    /// <summary>
    /// Invalida cache de um produto específico ou todos os produtos.
    /// </summary>
    Task InvalidateProductCache(Guid? productId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalida cache de produtos de uma categoria específica.
    /// </summary>
    Task InvalidateCategoryProductsCache(Guid categoryId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalida todos os caches de produto.
    /// </summary>
    Task InvalidateAllProductCaches(CancellationToken cancellationToken = default);
}

public class CacheInvalidator : ICacheInvalidator
{
    private readonly IDistributedCache _cache;

    public CacheInvalidator(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Invalida cache de um produto específico. Se productId for null, invalida todas as listagens paginadas.
    /// </summary>
    public async Task InvalidateProductCache(Guid? productId = null, CancellationToken cancellationToken = default)
    {
        if (productId.HasValue)
        {
            // Invalida apenas este produto específico
            await _cache.RemoveAsync(CacheKeys.ProductById(productId.Value), cancellationToken);
        }
        else
        {
            // Invalida todas as listagens paginadas (sem product ID específico)
            for (int page = 1; page <= 100; page++)  // Assumindo  100 páginas
            {
                for (int pageSize = 5; pageSize <= 100; pageSize += 5)
                {
                    await _cache.RemoveAsync(CacheKeys.ProductsPaged(page, pageSize), cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Invalida cache de produtos de uma categoria específica.
    /// </summary>
    public async Task InvalidateCategoryProductsCache(Guid categoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(CacheKeys.ProductsByCategory(categoryId), cancellationToken);
    }

    /// <summary>
    /// Invalida TODOS os caches de produto - usar com cuidado.
    /// </summary>
    public async Task InvalidateAllProductCaches(CancellationToken cancellationToken = default)
    {
        // Invalida listagens
        await InvalidateProductCache(null, cancellationToken);

    }
}
