using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

public record CreateCategoryCommand(string Name) : IRequest<Guid>;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _repository;
    public CreateCategoryHandler(ICategoryRepository repository) => _repository = repository;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name);
        await _repository.AddAsync(category);
        return category.Id;
    }
}