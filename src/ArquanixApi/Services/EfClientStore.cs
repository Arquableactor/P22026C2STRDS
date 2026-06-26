using ArquanixApi.Data;
using Arquanix.Domain.Entities;

namespace ArquanixApi.Services;


public class EfClientStore : IClientStore
{
    private readonly ArquanixDbContext _context;

    public EfClientStore(ArquanixDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Client> GetAll() => _context.Clients.OrderBy(c => c.Id).ToList();

    public Client? GetById(int id) => _context.Clients.Find(id);

    public Client Add(Client client)
    {
        client.CreatedAt = DateTime.UtcNow;
        _context.Clients.Add(client);
        _context.SaveChanges();
        return client;
    }

    public bool Update(Client client)
    {
        if (!_context.Clients.Any(c => c.Id == client.Id))
        {
            return false;
        }

        _context.Clients.Update(client);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var client = _context.Clients.Find(id);
        if (client is null)
        {
            return false;
        }

        _context.Clients.Remove(client);
        _context.SaveChanges();
        return true;
    }
}
