using Arquanix.Infrastructure.Context;
using Arquanix.Domain.Entities;

namespace ArquanixApi.Services;


public class EfClaimStore : IClaimStore
{
    private readonly ArquanixDbContext _context;

    public EfClaimStore(ArquanixDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Claim> GetAll() => _context.Claims.OrderBy(c => c.Id).ToList();

    public Claim? GetById(int id) => _context.Claims.Find(id);

    public Claim Add(Claim claim)
    {
        claim.CreatedAt = DateTime.UtcNow;
        _context.Claims.Add(claim);
        _context.SaveChanges();
        return claim;
    }

    public bool Update(Claim claim)
    {
        if (!_context.Claims.Any(c => c.Id == claim.Id))
        {
            return false;
        }

        _context.Claims.Update(claim);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var claim = _context.Claims.Find(id);
        if (claim is null)
        {
            return false;
        }

        _context.Claims.Remove(claim);
        _context.SaveChanges();
        return true;
    }
}
