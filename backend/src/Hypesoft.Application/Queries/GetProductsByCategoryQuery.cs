using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record GetProductsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<ProductResponse>>;

public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsByCategoryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetByCategoryIdAsync(request.CategoryId);
        
        return products.Select(p => new ProductResponse(
            p.Id, 
            p.Name, 
            p.Description, 
            p.Price, 
            p.StockQuantity, 
            p.CategoryId, 
            p.IsStockLow()
        ));
    }
}