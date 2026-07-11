namespace Arquanix.Application.Dtos.Clients;


public class CreateClientDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
}
