using Microsoft.AspNetCore.Mvc;

using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Services.Requests;
using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.V1.Clientes;

[Route("api/v1/cliente")]
[ApiController]
public class ClienteApiController : ControllerBase
{
    private readonly IClienteApiService _clienteApiService;

    public ClienteApiController(IClienteApiService clienteApiService)
    {
        _clienteApiService = clienteApiService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionResult<ClienteDto>>> Index([FromQuery] SearchClienteRequest query)
    {
        return Ok(await _clienteApiService.GetCollectionAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> Get(Guid id)
    {
        return Ok(await _clienteApiService.GetByIdAsync(new GetByKeyRequest<Guid> { Id = id }));
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Insert([FromBody] CreateUpdateClienteDto body)
    {
        var result = await _clienteApiService.CreateAsync(body);
        return CreatedAtAction(nameof(Get), new { Id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> Update(Guid id, [FromBody] CreateUpdateClienteDto body)
    {
        return Ok(await _clienteApiService.UpdateAsync(new GetByKeyRequest<Guid> { Id = id }, body));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _clienteApiService.DeleteAsync(new GetByKeyRequest<Guid> { Id = id });
        return Ok();
    }
}
