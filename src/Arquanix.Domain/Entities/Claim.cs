using Arquanix.Domain.Core;

namespace Arquanix.Domain.Entities;


public class Claim : BaseEntity
{
    public int ClientId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ClaimStatus Status { get; set; } = ClaimStatus.Open;

    public ClaimPriority Priority { get; set; } = ClaimPriority.Medium;

    public DateTime? ClosedAt { get; set; }
}
