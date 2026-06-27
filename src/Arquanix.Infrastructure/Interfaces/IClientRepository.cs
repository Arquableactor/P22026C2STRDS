using Arquanix.Domain.Entities;

namespace Arquanix.Infrastructure.Interfaces;


public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();

    Task<Client?> GetByIdAsync(int id);

    Task<Client> CreateAsync(Client client);

    Task<bool> UpdateAsync(Client client);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}
