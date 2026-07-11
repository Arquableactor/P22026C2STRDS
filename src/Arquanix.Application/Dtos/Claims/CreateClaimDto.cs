using Arquanix.Domain.Entities;

namespace Arquanix.Application.Dtos.Claims;


public class CreateClaimDto
{
    public int ClientId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ClaimStatus Status { get; set; } = ClaimStatus.Open;

    public ClaimPriority Priority { get; set; } = ClaimPriority.Medium;
}
