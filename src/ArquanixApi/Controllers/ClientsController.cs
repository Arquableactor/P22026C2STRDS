using ArquanixApi.Dtos;
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
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ClientDto>> GetAll()
    {
        return Ok(_store.GetAll().Select(ToDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ClientDto> GetById(int id)
    {
        var client = _store.GetById(id);
        return client is null ? NotFound() : Ok(ToDto(client));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ClientDto> Create([FromBody] CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            IsActive = dto.IsActive,
        };

        var created = _store.Add(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateClientDto dto)
    {
        var existing = _store.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.IsActive = dto.IsActive;

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

    private static ClientDto ToDto(Client client) => new()
    {
        Id = client.Id,
        Name = client.Name,
        Email = client.Email,
        Phone = client.Phone,
        IsActive = client.IsActive,
        CreatedAt = client.CreatedAt,
    };
}
