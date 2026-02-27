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
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return false;
            
        await _repository.DeleteAsync(request.Id, cancellationToken);
        Log.Warning("Produto {ProductId} removido do sistema", request.Id);
        return true;
    }
}