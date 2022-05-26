using Microsoft.AspNetCore.Mvc;

using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Services.Requests;
using Poc.EventDriven.Services.Results;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poc.EventDriven.V1.Boms
{
    [Route("api/v1/bom")]
    [ApiController]
    public class BomApiController : ControllerBase
    {
        private readonly IBomApiService _bomsApiService;

        public BomApiController(IBomApiService bomsApiService)
        {
            _bomsApiService = bomsApiService;
        }

        [HttpGet]
        public async Task<ActionResult<CollectionResult<BomDto>>> Index([FromQuery] SearchBomRequest query)
        {
            return Ok(await _bomsApiService.GetCollectionAsync(query));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BomDto>> Get(Guid id)
        {
            return Ok(await _bomsApiService.GetByIdAsync(new GetByKeyRequest<Guid> { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult<BomDto>> Post([FromBody] CreateUpdateBomDto body)
        {
            var bom = await _bomsApiService.CreateAsync(body);
            return CreatedAtAction(nameof(Get), new { Id = bom.Id }, bom);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BomDto>> Put(Guid id, [FromBody] CreateUpdateBomDto body)
        {
            return Ok(await _bomsApiService.UpdateAsync(new GetByKeyRequest<Guid> { Id = id }, body));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _bomsApiService.DeleteAsync(new GetByKeyRequest<Guid> { Id = id });
            return Ok();
        }
    }
}
