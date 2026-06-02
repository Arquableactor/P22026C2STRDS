using ArquanixApi.Models;

namespace ArquanixApi.Services;


public interface IClientStore
{
    IEnumerable<Client> GetAll();

    Client? GetById(int id);

    Client Add(Client client);

    bool Update(Client client);

    bool Delete(int id);
}
