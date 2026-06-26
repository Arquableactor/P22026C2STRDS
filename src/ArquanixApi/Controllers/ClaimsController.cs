using ArquanixApi.Dtos;
using Arquanix.Domain.Entities;
using ArquanixApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArquanixApi.Controllers;

[ApiController]
[Route("api/claims")]
[Produces("application/json")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimStore _store;
    private readonly IClientStore _clientStore;

    public ClaimsController(IClaimStore store, IClientStore clientStore)
    {
        _store = store;
        _clientStore = clientStore;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ClaimDto>> GetAll()
    {
        return Ok(_store.GetAll().Select(ToDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ClaimDto> GetById(int id)
    {
        var claim = _store.GetById(id);
        return claim is null ? NotFound() : Ok(ToDto(claim));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ClaimDto> Create([FromBody] CreateClaimDto dto)
    {
        if (_clientStore.GetById(dto.ClientId) is null)
        {
            ModelState.AddModelError(nameof(dto.ClientId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var claim = new Claim
        {
            ClientId = dto.ClientId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            ClosedAt = dto.Status == ClaimStatus.Closed ? DateTime.UtcNow : null,
        };

        var created = _store.Add(claim);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateClaimDto dto)
    {
        var existing = _store.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (_clientStore.GetById(dto.ClientId) is null)
        {
            ModelState.AddModelError(nameof(dto.ClientId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        existing.ClientId = dto.ClientId;
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Priority = dto.Priority;
        existing.Status = dto.Status;
        existing.ClosedAt = dto.Status == ClaimStatus.Closed
            ? existing.ClosedAt ?? DateTime.UtcNow
            : null;

        _store.Update(existing);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        return _store.Delete(id) ? NoContent() : NotFound();
    }

    private static ClaimDto ToDto(Claim claim) => new()
    {
        Id = claim.Id,
        ClientId = claim.ClientId,
        Title = claim.Title,
        Description = claim.Description,
        Status = claim.Status,
        Priority = claim.Priority,
        CreatedAt = claim.CreatedAt,
        ClosedAt = claim.ClosedAt,
    };
}
