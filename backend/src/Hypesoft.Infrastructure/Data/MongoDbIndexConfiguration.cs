using MongoDB.Driver;
using Hypesoft.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Hypesoft.Infrastructure.Data;

/// <summary>
/// Configuração de índices do MongoDB para otimização de performance.
/// Aplica índices estratégicos baseados nos padrões de query da aplicação.
/// </summary>
public static class MongoDbIndexConfiguration
{
    /// <summary>
    /// Cria todos os índices necessários nas collections do MongoDB.
    /// </summary>
    public static async Task EnsureIndexesAsync(IMongoDatabase database, ILogger? logger = null)
    {
        await CreateProductIndexesAsync(database, logger);
        await CreateCategoryIndexesAsync(database, logger);
    }

    /// <summary>
    /// Cria índices otimizados para a collection de Products.
    /// </summary>
    private static async Task CreateProductIndexesAsync(IMongoDatabase database, ILogger? logger = null)
    {
        var collection = database.GetCollection<Product>("products");
        var indexKeysDefinition = Builders<Product>.IndexKeys;

        logger?.LogInformation("Criando índices para collection 'products'...");
        // 1. Índice em CategoryId 
        var categoryIdIndex = new CreateIndexModel<Product>(
            indexKeysDefinition.Ascending(p => p.CategoryId),
            new CreateIndexOptions 
            { 
                Name = "idx_categoryid",
                Background = true 
            }
        );

        // 2. Índice em StockQuantity 
        var stockQuantityIndex = new CreateIndexModel<Product>(
            indexKeysDefinition.Ascending(p => p.StockQuantity),
            new CreateIndexOptions 
            { 
                Name = "idx_stockquantity",
                Background = true 
            }
        );

        // 3. Índice text search em Name (
        var nameTextIndex = new CreateIndexModel<Product>(
            indexKeysDefinition.Text(p => p.Name),
            new CreateIndexOptions 
            { 
                Name = "idx_name_text",
                Background = true,
                DefaultLanguage = "portuguese"
            }
        );

        // 4. Índice composto CategoryId + StockQuantity

        var categoryStockIndex = new CreateIndexModel<Product>(
            indexKeysDefinition
                .Ascending(p => p.CategoryId)
                .Ascending(p => p.StockQuantity),
            new CreateIndexOptions 
            { 
                Name = "idx_category_stock",
                Background = true 
            }
        );

        // 5. Índice em Price (para ordenação e filtros por preço)
        var priceIndex = new CreateIndexModel<Product>(
            indexKeysDefinition.Ascending(p => p.Price),
            new CreateIndexOptions 
            { 
                Name = "idx_price",
                Background = true 
            }
        );

        // 6. Índice composto para cálculos de valor total em estoque
        var priceStockIndex = new CreateIndexModel<Product>(
            indexKeysDefinition
                .Ascending(p => p.Price)
                .Ascending(p => p.StockQuantity),
            new CreateIndexOptions 
            { 
                Name = "idx_price_stock",
                Background = true 
            }
        );

        try
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                categoryIdIndex,
                stockQuantityIndex,
                nameTextIndex,
                categoryStockIndex,
                priceIndex,
                priceStockIndex
            });

            logger?.LogInformation("✓ Índices de 'products' criados com sucesso");
        }
        catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict")
        {
            // Índices já existem, ignorar erro
            logger?.LogInformation("Índices de 'products' já existem");
        }
    }

    /// <summary>
    /// Cria índices otimizados para a collection de Categories.
    /// </summary>
    private static async Task CreateCategoryIndexesAsync(IMongoDatabase database, ILogger? logger = null)
    {
        var collection = database.GetCollection<Category>("categories");
        var indexKeysDefinition = Builders<Category>.IndexKeys;

        logger?.LogInformation("Criando índices para collection 'categories'...");

        // 1. Índice único em Name 
        var nameUniqueIndex = new CreateIndexModel<Category>(
            indexKeysDefinition.Ascending(c => c.Name),
            new CreateIndexOptions 
            { 
                Name = "idx_name_unique",
                Unique = true,
                Background = true 
            }
        );

        // 2. Índice text search em Name (para futuras buscas)
        var nameTextIndex = new CreateIndexModel<Category>(
            indexKeysDefinition.Text(c => c.Name),
            new CreateIndexOptions 
            { 
                Name = "idx_category_name_text",
                Background = true,
                DefaultLanguage = "portuguese"
            }
        );

        try
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                nameUniqueIndex,
                nameTextIndex
            });

            logger?.LogInformation("✓ Índices de 'categories' criados com sucesso");
        }
        catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict")
        {
            // Índices já existem, ignorar erro
            logger?.LogInformation("Índices de 'categories' já existem");
        }
    }

    /// <summary>
    /// Lista todos os índices existentes em uma collection (útil para debug).
    /// </summary>
    public static async Task<List<string>> GetExistingIndexesAsync<T>(IMongoCollection<T> collection)
    {
        var indexes = new List<string>();
        using var cursor = await collection.Indexes.ListAsync();
        var indexDocuments = await cursor.ToListAsync();
        
        foreach (var doc in indexDocuments)
        {
            if (doc.TryGetValue("name", out var name))
            {
                indexes.Add(name.ToString() ?? "unknown");
            }
        }
        
        return indexes;
    }
}
