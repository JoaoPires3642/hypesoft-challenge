namespace Hypesoft.Application.DTOs;

public record DashboardResponse(
    int TotalProducts,
    decimal TotalStockValue,
    IEnumerable<ProductResponse> LowStockProducts,
    IEnumerable<CategoryChartData> ProductsByCategory
);

public record CategoryChartData(string CategoryName, int ProductCount);