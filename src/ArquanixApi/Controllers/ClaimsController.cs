using ArquanixApi.Dtos;
using Arquanix.Domain.Entities;
using Arquanix.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArquanixApi.Controllers;

[ApiController]
[Route("api/claims")]
[Produces("application/json")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimRepository _repository;
    private readonly IClientRepository _clientRepository;

    public ClaimsController(IClaimRepository repository, IClientRepository clientRepository)
    {
        _repository = repository;
        _clientRepository = clientRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAll()
    {
        var claims = await _repository.GetAllAsync();
        return Ok(claims.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetById(int id)
    {
        var claim = await _repository.GetByIdAsync(id);
        return claim is null ? NotFound() : Ok(ToDto(claim));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> Create([FromBody] CreateClaimDto dto)
    {
        if (!await _clientRepository.ExistsAsync(dto.ClientId))
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

        var created = await _repository.CreateAsync(claim);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClaimDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (!await _clientRepository.ExistsAsync(dto.ClientId))
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

        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        return await _repository.DeleteAsync(id) ? NoContent() : NotFound();
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
