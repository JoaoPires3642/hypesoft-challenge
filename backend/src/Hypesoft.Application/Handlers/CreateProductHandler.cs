using MediatR;
using Serilog;
using Hypesoft.Application.Commands;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheInvalidator _cacheInvalidator;

    public CreateProductHandler(IProductRepository productRepository, ICacheInvalidator cacheInvalidator)
    {
        _productRepository = productRepository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Iniciando criação do produto: {ProductName}", request.Name);
        try
        {
            var product = new Product(
                request.Name, 
                request.Description, 
                request.Price, 
                request.StockQuantity, 
                request.CategoryId
            );

            await _productRepository.AddAsync(product, cancellationToken);
            await _cacheInvalidator.InvalidateProductCache(null, cancellationToken);
            await _cacheInvalidator.InvalidateCategoryProductsCache(product.CategoryId, cancellationToken);

            Log.Information("Produto {ProductName} criado com sucesso. ID: {ProductId}", 
                product.Name, product.Id);

            return product.Id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao criar produto {ProductName}", request.Name);
            throw;
        }
    }
}
