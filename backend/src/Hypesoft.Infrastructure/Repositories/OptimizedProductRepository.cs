using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Constants;
using Hypesoft.Infrastructure.Data;

namespace Hypesoft.Infrastructure.Repositories;

/// <summary>
/// Repositório de produtos com queries otimizadas usando índices do MongoDB.
/// </summary>
public class OptimizedProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly IMongoCollection<Product>? _collection;

    public OptimizedProductRepository(AppDbContext context)
    {
        _context = context;
        
        // Obter collection nativa do MongoDB para queries otimizadas
        var database = context.Database.GetService<IMongoDatabase>();
        _collection = database?.GetCollection<Product>("products");
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Busca produtos por nome usando text search do MongoDB (índice otimizado).
    /// Fallback para Contains se text search não estiver disponível.
    /// </summary>
    public async Task<IEnumerable<Product>> SearchAsync(string? name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync(cancellationToken);
        }

        // Tentar usar text search do MongoDB (mais otimizado)
        if (_collection != null)
        {
            try
            {
                var filter = Builders<Product>.Filter.Text(name);
                var results = await _collection
                    .Find(filter)
                    .ToListAsync(cancellationToken);
                
                if (results.Any())
                {
                    return results;
                }
            }
            catch
            {
                // Text index pode não estar disponível ainda, continuar com fallback
            }
        }

        // Fallback: usar EF Core com Contains (menos otimizado mas funciona sempre)
        var lowerName = name.ToLower();
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(lowerName))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém produtos por categoria usando índice otimizado.
    /// </summary>
    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        // Query otimizada com índice em CategoryId
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém produtos com estoque baixo usando índice otimizado.
    /// </summary>
    public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold = ProductConstants.LOW_STOCK_THRESHOLD, CancellationToken cancellationToken = default)
    {
        // Query otimizada com índice em StockQuantity
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.StockQuantity < threshold)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Paginação otimizada com AsNoTracking.
    /// </summary>
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Products.AsNoTracking();
        
        // CountAsync é otimizado com índices
        var totalCount = await query.CountAsync();
        
        // Skip/Take usa índices internos do MongoDB
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
