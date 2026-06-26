using Arquanix.Domain.Entities;

namespace ArquanixApi.Dtos;


public class ClaimDto
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ClaimStatus Status { get; set; }

    public ClaimPriority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
