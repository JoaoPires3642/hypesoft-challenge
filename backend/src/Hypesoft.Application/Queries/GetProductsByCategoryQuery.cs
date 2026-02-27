using AutoMapper;
using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries;

public record GetProductsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<ProductResponse>>;

public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsByCategoryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }
}