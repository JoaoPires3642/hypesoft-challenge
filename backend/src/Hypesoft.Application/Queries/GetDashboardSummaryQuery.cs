using AutoMapper;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Queries;
using Hypesoft.Domain.Constants;
using MediatR;

namespace Hypesoft.Application.Queries;

public record GetDashboardSummaryQuery() : IRequest<DashboardResponse>;

public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardResponse>
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IProductQueryService _queryService;
    private readonly IMapper _mapper;

    public GetDashboardSummaryHandler(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductQueryService queryService, IMapper mapper)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _queryService = queryService;
        _mapper = mapper;
    }

    public async Task<DashboardResponse> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalCountTask = _queryService.GetTotalCountAsync(cancellationToken);
        var totalValueTask = _queryService.GetTotalStockValueAsync(cancellationToken);
        var lowStockTask = _productRepo.GetLowStockAsync(ProductConstants.LOW_STOCK_THRESHOLD, cancellationToken);
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
            _mapper.Map<IEnumerable<ProductResponse>>(lowStockTask.Result),
            chartData
        );
    }
}