using ArquanixApi.Models;
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
    [ProducesResponseType(typeof(IEnumerable<Claim>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Claim>> GetAll()
    {
        return Ok(_store.GetAll());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Claim> GetById(int id)
    {
        var claim = _store.GetById(id);
        return claim is null ? NotFound() : Ok(claim);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Claim> Create([FromBody] ClaimRequest request)
    {
        if (_clientStore.GetById(request.ClientId) is null)
        {
            ModelState.AddModelError(nameof(request.ClientId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        var claim = new Claim
        {
            ClientId = request.ClientId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            ClosedAt = request.Status == ClaimStatus.Closed ? DateTime.UtcNow : null,
        };

        var created = _store.Add(claim);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ClaimRequest request)
    {
        var existing = _store.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (_clientStore.GetById(request.ClientId) is null)
        {
            ModelState.AddModelError(nameof(request.ClientId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        existing.ClientId = request.ClientId;
        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Priority = request.Priority;
        existing.Status = request.Status;
        existing.ClosedAt = request.Status == ClaimStatus.Closed
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
}
