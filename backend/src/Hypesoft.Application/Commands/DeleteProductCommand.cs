using Hypesoft.Domain.Repositories;
using MediatR;
using Serilog;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    public DeleteProductHandler(IProductRepository repository) => _repository = repository;

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        Log.Warning("Produto {ProductId} removido do sistema", request.Id);
        return true;
    }
}