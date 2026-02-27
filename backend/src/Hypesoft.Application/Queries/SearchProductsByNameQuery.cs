using AutoMapper;
using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record SearchProductsByNameQuery(string Name) : IRequest<IEnumerable<ProductResponse>>;

public class SearchProductsByNameHandler : IRequestHandler<SearchProductsByNameQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public SearchProductsByNameHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(SearchProductsByNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.SearchAsync(request.Name, cancellationToken);
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
}