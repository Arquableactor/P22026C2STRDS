using System.Collections.Concurrent;
using ArquanixApi.Models;

namespace ArquanixApi.Services;


public class InMemoryClaimStore : IClaimStore
{
    private readonly ConcurrentDictionary<int, Claim> _claims = new();
    private int _lastId;

    public IEnumerable<Claim> GetAll() => _claims.Values.OrderBy(c => c.Id);

    public Claim? GetById(int id) => _claims.TryGetValue(id, out var claim) ? claim : null;

    public Claim Add(Claim claim)
    {
        claim.Id = Interlocked.Increment(ref _lastId);
        claim.CreatedAt = DateTime.UtcNow;
        _claims[claim.Id] = claim;
        return claim;
    }

    public bool Update(Claim claim)
    {
        if (!_claims.ContainsKey(claim.Id))
        {
            return false;
        }

        _claims[claim.Id] = claim;
        return true;
    }

    public bool Delete(int id) => _claims.TryRemove(id, out _);
}
