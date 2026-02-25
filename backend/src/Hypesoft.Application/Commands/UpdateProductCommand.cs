using MediatR;
using Hypesoft.Domain.Repositories;
using Serilog;

namespace Hypesoft.Application.Commands;

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, int StockQuantity) : IRequest<bool>;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _repository;
    public UpdateProductHandler(IProductRepository repository) => _repository = repository;

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) return false;

        // Aqui você usaria métodos da entidade para atualizar (DDD)
        // product.UpdateDetails(request.Name, request.Description, request.Price);
        // product.UpdateStock(request.StockQuantity);

        await _repository.UpdateAsync(product);
        Log.Information("Produto {ProductId} atualizado com sucesso", product.Id);
        return true;
    }
}