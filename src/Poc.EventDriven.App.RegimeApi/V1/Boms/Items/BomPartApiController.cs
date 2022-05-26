using Microsoft.AspNetCore.Mvc;

using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.V1.Boms.Items;

[Route("api/v1/bom/{bomId:guid}/item")]
[ApiController]
public class BomPartApiController : ControllerBase
{
    private readonly IBomPartApiService _bomPartApiService;

    public BomPartApiController(IBomPartApiService bomPartApiService)
    {
        _bomPartApiService = bomPartApiService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionResult<BomPart>>> Index(Guid bomId, [FromQuery] SearchBomPartRequest query)
    {
        query.BomId = bomId;
        return Ok(await _bomPartApiService.GetCollectionAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BomPartDto>> Get(Guid id, Guid bomId)
    {
        return Ok(await _bomPartApiService.GetByIdAsync(new GetBomPartByKey { Id = id, BomId = bomId }));
    }

    [HttpPost]
    public async Task<ActionResult<BomPartDto>> Insert(Guid bomId, [FromBody] BomPartCreateUpdateBody body)
    {
        var bomPart = await _bomPartApiService.CreateAsync(new CreateUpdateBomPartDto
        {
            BomId = bomId,
            MaterialId = body.MaterialId,
            Quantidade = body.Quantidade
        });
        return CreatedAtAction(nameof(Get), new { Id = bomPart.Id, BomId = bomId }, bomPart);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BomPartDto>> Update(Guid bomId, Guid id, [FromBody] BomPartCreateUpdateBody body)
    {
        return Ok(await _bomPartApiService.UpdateAsync(
            new GetBomPartByKey { BomId = bomId, Id = id },
            new CreateUpdateBomPartDto
            {
                BomId = bomId,
                MaterialId = body.MaterialId,
                Quantidade = body.Quantidade
            }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid bomId, Guid id)
    {
        await _bomPartApiService.DeleteAsync(new GetBomPartByKey { BomId = bomId, Id = id });
        return Ok();
    }
}
