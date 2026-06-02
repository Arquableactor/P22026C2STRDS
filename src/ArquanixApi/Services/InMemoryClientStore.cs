using System.Collections.Concurrent;
using ArquanixApi.Models;

namespace ArquanixApi.Services;


public class InMemoryClientStore : IClientStore
{
    private readonly ConcurrentDictionary<int, Client> _clients = new();
    private int _lastId;

    public IEnumerable<Client> GetAll() => _clients.Values.OrderBy(c => c.Id);

    public Client? GetById(int id) => _clients.TryGetValue(id, out var client) ? client : null;

    public Client Add(Client client)
    {
        client.Id = Interlocked.Increment(ref _lastId);
        client.CreatedAt = DateTime.UtcNow;
        _clients[client.Id] = client;
        return client;
    }

    public bool Update(Client client)
    {
        if (!_clients.ContainsKey(client.Id))
        {
            return false;
        }

        _clients[client.Id] = client;
        return true;
    }

    public bool Delete(int id) => _clients.TryRemove(id, out _);
}
