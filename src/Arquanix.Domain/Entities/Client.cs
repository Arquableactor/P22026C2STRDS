using Arquanix.Domain.Core;

namespace Arquanix.Domain.Entities;


public class Client : Person
{
    public override string GetSummary() => $"Cliente #{Id}: {Name} <{Email}>";
}
