using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record GetProductsQuery() : IRequest<IEnumerable<ProductResponse>>;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsHandler(IProductRepository repository) => _repository = repository;

    public async Task<IEnumerable<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductResponse(
            p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, p.IsStockLow()
        ));
    }
}