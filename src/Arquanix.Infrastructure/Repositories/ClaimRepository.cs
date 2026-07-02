using Arquanix.Domain.Entities;
using Arquanix.Infrastructure.Context;
using Arquanix.Infrastructure.Core;
using Arquanix.Infrastructure.Interfaces;

namespace Arquanix.Infrastructure.Repositories;


public class ClaimRepository : BaseRepository<Claim>, IClaimRepository
{
    public ClaimRepository(ArquanixDbContext context)
        : base(context)
    {
    }
}
