using Hypesoft.Application.DTOs;
using Hypesoft.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hypesoft.API.Controllers;

/// <summary>
/// Controller para dados agregados do dashboard.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="DashboardController"/>.
    /// </summary>
    /// <param name="mediator">Mediator para despacho de queries.</param>
    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retorna os dados consolidados do dashboard.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Retorna os dados consolidados do dashboard")]
    public async Task<ActionResult<DashboardResponse>> GetSummary()
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery());
        return Ok(result);
    }
}