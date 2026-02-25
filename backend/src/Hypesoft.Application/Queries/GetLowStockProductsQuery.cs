using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record GetLowStockProductsQuery() : IRequest<IEnumerable<ProductResponse>>;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;
    public GetLowStockProductsHandler(IProductRepository repository) => _repository = repository;

    public async Task<IEnumerable<ProductResponse>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetLowStockAsync(10, cancellationToken);
        return products.Select(p => new ProductResponse(
            p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, p.IsStockLow()
        ));
    }
}