using ArquanixApi.Models;
using ArquanixApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArquanixApi.Controllers;

[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IClientStore _store;

    public ClientsController(IClientStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Client>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Client>> GetAll()
    {
        return Ok(_store.GetAll());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Client> GetById(int id)
    {
        var client = _store.GetById(id);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Client> Create([FromBody] ClientRequest request)
    {
        var client = new Client
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = request.IsActive,
        };

        var created = _store.Add(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ClientRequest request)
    {
        var existing = _store.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = request.Name;
        existing.Email = request.Email;
        existing.Phone = request.Phone;
        existing.IsActive = request.IsActive;

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
