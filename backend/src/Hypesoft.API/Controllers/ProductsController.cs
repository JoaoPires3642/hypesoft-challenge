using Hypesoft.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hypesoft.API.Controllers;

/// <summary>
/// Controller para gerenciar produtos no sistema Hypesoft
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo produto no sistema
    /// </summary>
    /// <remarks>
    /// ## Exemplo de Requisição:
    /// 
    ///     POST /api/products
    ///     {
    ///       "name": "Notebook Dell",
    ///       "description": "Notebook de alta performance",
    ///       "price": 4500.00,
    ///       "categoryId": "categoria-001"
    ///     }
    /// 
    /// ## Exemplo de Resposta (201 Created):
    /// 
    ///     {
    ///       "productId": "produto-001"
    ///     }
    /// </remarks>
    /// <param name="command">Dados do produto a ser criado</param>
    /// <returns>ID do produto criado</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Criar novo produto",
        Description = "Cria um novo produto com as informações fornecidas"
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }
}