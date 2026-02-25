using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Queries;

public record GetProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResponse<ProductResponse>>;