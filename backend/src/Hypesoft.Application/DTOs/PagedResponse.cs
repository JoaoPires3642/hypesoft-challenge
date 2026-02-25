namespace Hypesoft.Application.DTOs;

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}