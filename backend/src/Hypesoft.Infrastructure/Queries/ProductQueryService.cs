using Microsoft.EntityFrameworkCore;
using Hypesoft.Domain.Queries;
using Hypesoft.Infrastructure.Data;

namespace Hypesoft.Infrastructure.Queries;

/// <summary>
/// Implementação do serviço de consultas de produtos com agregações e relatórios.
/// </summary>
public class ProductQueryService : IProductQueryService
{
    private readonly AppDbContext _context;

    public ProductQueryService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém a contagem total de produtos no sistema.
    /// </summary>
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(cancellationToken);

    /// <summary>
    /// Calcula o valor total em estoque (preço * quantidade).
    /// Utiliza AsNoTracking para melhor performance em queries de leitura.
    /// </summary>
    public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .SumAsync(p => p.Price * p.StockQuantity, cancellationToken);
    }

    /// <summary>
    /// Obtém a contagem de produtos agrupada por categoria.
    /// </summary>
    public async Task<Dictionary<Guid, int>> GetCountByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .GroupBy(p => p.CategoryId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }
}
