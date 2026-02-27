namespace Hypesoft.Application.Infrastructure.Cache;

/// <summary>
/// Define as chaves de cache padronizadas para o sistema.
/// </summary>
public static class CacheKeys
{
    public const string PRODUCTS_PREFIX = "products";
    public const string PRODUCTS_PAGED = $"{PRODUCTS_PREFIX}:paged";
    public const string PRODUCTS_BY_CATEGORY = $"{PRODUCTS_PREFIX}:category";
    public const string PRODUCT_BY_ID = $"{PRODUCTS_PREFIX}:id";
    
    /// <summary>
    /// Gera chave para produtos paginados.
    /// </summary>
    public static string ProductsPaged(int page, int pageSize) => $"{PRODUCTS_PAGED}:{page}:{pageSize}";
    
    /// <summary>
    /// Gera chave para produtos de uma categoria específica.
    /// </summary>
    public static string ProductsByCategory(Guid categoryId) => $"{PRODUCTS_BY_CATEGORY}:{categoryId}";
    
    /// <summary>
    /// Gera chave para um produto específico.
    /// </summary>
    public static string ProductById(Guid productId) => $"{PRODUCT_BY_ID}:{productId}";
}
