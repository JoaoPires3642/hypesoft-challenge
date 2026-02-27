using AutoMapper;
using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record GetLowStockProductsQuery() : IRequest<IEnumerable<ProductResponse>>;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    
    public GetLowStockProductsHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetLowStockAsync(10, cancellationToken);
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
}