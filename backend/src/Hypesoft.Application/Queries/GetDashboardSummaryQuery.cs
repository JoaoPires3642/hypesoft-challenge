using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Queries;
using MediatR;

namespace Hypesoft.Application.Queries;

public record GetDashboardSummaryQuery() : IRequest<DashboardResponse>;

public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardResponse>
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IProductQueryService _queryService;

    public GetDashboardSummaryHandler(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductQueryService queryService)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _queryService = queryService;
    }

    public async Task<DashboardResponse> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalCountTask = _queryService.GetTotalCountAsync(cancellationToken);
        var totalValueTask = _queryService.GetTotalStockValueAsync(cancellationToken);
        var lowStockTask = _productRepo.GetLowStockAsync(10, cancellationToken);
        var categoryCountsTask = _queryService.GetCountByCategoryAsync(cancellationToken);
        var categoriesTask = _categoryRepo.GetAllAsync();

        // Aguarda todas as tarefas 
        await Task.WhenAll(totalCountTask, totalValueTask, lowStockTask, categoryCountsTask, categoriesTask);

        
        var chartData = categoriesTask.Result.Select(c => new CategoryChartData(
            c.Name,
            categoryCountsTask.Result.GetValueOrDefault(c.Id, 0)
        ));

        return new DashboardResponse(
            totalCountTask.Result,
            totalValueTask.Result,
            lowStockTask.Result.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, true)),
            chartData
        );
    }
}