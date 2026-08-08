using Arquanix.Application.Contract;
using Arquanix.Application.Dtos.Claims;
using Arquanix.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ArquanixApi.Controllers;

[ApiController]
[Route("api/claims")]
[Produces("application/json")]
public class ClaimsController : ApiControllerBase
{
    private readonly IClaimService _service;

    public ClaimsController(IClaimService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAll([FromQuery] int? clientId, [FromQuery] ClaimStatus? status)
    {
        var result = await _service.GetAllAsync(clientId, status);
        return Ok(result.Data);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ClaimStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClaimStatsDto>> GetStats()
    {
        var result = await _service.GetStatsAsync();
        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> Create([FromBody] CreateClaimDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Success)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClaimDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.Success ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? NoContent() : HandleFailure(result);
    }
}
