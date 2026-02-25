using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

public record GetCategoriesQuery() : IRequest<IEnumerable<Category>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<Category>>
{
    private readonly ICategoryRepository _repository;
    public GetCategoriesHandler(ICategoryRepository repository) => _repository = repository;

    public async Task<IEnumerable<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        => await _repository.GetAllAsync();
}