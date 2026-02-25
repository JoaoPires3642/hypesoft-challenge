using MediatR;
using Serilog;
using Hypesoft.Application.Commands;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Hypesoft.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IDistributedCache _cache;

    public CreateProductHandler(IProductRepository productRepository, IDistributedCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
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
            await _cache.RemoveAsync("products_all_");
            

            Log.Information("Produto {ProductName} criado com sucesso. ID: {ProductId}", 
                product.Name, product.Id);

            return product.Id;
;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao criar produto {ProductName}", request.Name);
            throw;
        }
    }
}