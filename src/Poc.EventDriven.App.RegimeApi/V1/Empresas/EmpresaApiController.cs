using Microsoft.AspNetCore.Mvc;

using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Services.Requests;
using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.V1.Empresas;

[Route("api/v1/empresa")]
[ApiController]
public class EmpresaApiController : ControllerBase
{
    private readonly IEmpresaApiService _empresaApiService;

    public EmpresaApiController(IEmpresaApiService clienteApiService)
    {
        _empresaApiService = clienteApiService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionResult<EmpresaDto>>> Index([FromQuery] SearchEmpresaRequest query)
    {
        return Ok(await _empresaApiService.GetCollectionAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmpresaDto>> Get(Guid id)
    {
        return Ok(await _empresaApiService.GetByIdAsync(new GetByKeyRequest<Guid> { Id = id }));
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaDto>> Insert([FromBody] CreateUpdateEmpresaDto body)
    {
        var result = await _empresaApiService.CreateAsync(body);
        return CreatedAtAction(nameof(Get), new { Id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmpresaDto>> Update(Guid id, [FromBody] CreateUpdateEmpresaDto body)
    {
        return Ok(await _empresaApiService.UpdateAsync(new GetByKeyRequest<Guid> { Id = id }, body));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _empresaApiService.DeleteAsync(new GetByKeyRequest<Guid> { Id = id });
        return Ok();
    }
}
