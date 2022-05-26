using Microsoft.AspNetCore.Mvc;

using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Services.Requests;
using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.V1.Produtos;

[Route("api/v1/produto")]
[ApiController]
public class ProdutoApiController : ControllerBase
{
    private readonly IProdutoApiService _produtoApiService;

    public ProdutoApiController(IProdutoApiService clienteApiService)
    {
        _produtoApiService = clienteApiService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionResult<ProdutoDto>>> Index([FromQuery] SearchProdutoRequest query)
    {
        return Ok(await _produtoApiService.GetCollectionAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoDto>> Get(Guid id)
    {
        return Ok(await _produtoApiService.GetByIdAsync(new GetByKeyRequest<Guid> { Id = id }));
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Insert([FromBody] CreateUpdateProdutoDto body)
    {
        var result = await _produtoApiService.CreateAsync(body);
        return CreatedAtAction(nameof(Get), new { Id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProdutoDto>> Update(Guid id, [FromBody] CreateUpdateProdutoDto body)
    {
        return Ok(await _produtoApiService.UpdateAsync(new GetByKeyRequest<Guid> { Id = id }, body));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _produtoApiService.DeleteAsync(new GetByKeyRequest<Guid> { Id = id });
        return Ok();
    }
}
