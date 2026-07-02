using Arquanix.Domain.Entities;
using Arquanix.Infrastructure.Context;
using Arquanix.Infrastructure.Core;
using Arquanix.Infrastructure.Interfaces;

namespace Arquanix.Infrastructure.Repositories;


public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(ArquanixDbContext context)
        : base(context)
    {
    }
}
