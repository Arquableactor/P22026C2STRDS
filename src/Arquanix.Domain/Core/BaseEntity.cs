namespace Arquanix.Domain.Core;


public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public abstract string GetSummary();
}
