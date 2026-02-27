namespace Hypesoft.Domain.Queries;

/// <summary>
/// Serviço de consultas e agregações de produtos.
/// Responsável por relatórios e queries de leitura que envolvem agregações.
/// </summary>
public interface IProductQueryService
{
    /// <summary>
    /// Obtém a contagem total de produtos no sistema.
    /// </summary>
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o valor total em estoque (preço * quantidade).
    /// </summary>
    Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a contagem de produtos agrupada por categoria.
    /// </summary>
    Task<Dictionary<Guid, int>> GetCountByCategoryAsync(CancellationToken cancellationToken = default);
}
