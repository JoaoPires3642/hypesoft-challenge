using Hypesoft.Application.Commands;
using Hypesoft.Application.Queries;
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
    /// Retorna todos os produtos cadastrados
    /// </summary>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar produtos", Description = "Retorna todos os produtos cadastrados")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        return Ok(products);
    }

    /// <summary>
    /// Retorna um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <response code="200">Produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Buscar produto por ID", Description = "Retorna um produto específico pelo ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
       var products = await _mediator.Send(new GetProductsQuery());
        var product = products.FirstOrDefault(p => p.Id == id);
        return product != null ? Ok(product) : NotFound();
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
    /// </remarks>
    /// <param name="command">Dados do produto</param>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Criar produto", Description = "Cria um novo produto")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="command">Dados atualizados do produto</param>
    /// <response code="204">Produto atualizado com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar produto", Description = "Atualiza os dados de um produto existente")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID da rota diferente do ID do corpo da requisição.");

        var success = await _mediator.Send(command);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Remove um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <response code="204">Produto removido com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Excluir produto", Description = "Remove um produto pelo ID")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id));

        if (!success)
            return NotFound();

        return NoContent();
    }
}