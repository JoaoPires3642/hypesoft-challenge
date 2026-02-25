using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record SearchProductsByNameQuery(string Name) : IRequest<IEnumerable<ProductResponse>>;

public class SearchProductsByNameHandler : IRequestHandler<SearchProductsByNameQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public SearchProductsByNameHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(SearchProductsByNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.SearchAsync(request.Name);
        
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