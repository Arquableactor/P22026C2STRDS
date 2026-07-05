using Arquanix.Domain.Entities;

namespace Arquanix.Application.Dtos.Claims;


public class ClaimDto : DtoBase
{
    public int ClientId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ClaimStatus Status { get; set; }

    public ClaimPriority Priority { get; set; }

    public DateTime? ClosedAt { get; set; }
}
