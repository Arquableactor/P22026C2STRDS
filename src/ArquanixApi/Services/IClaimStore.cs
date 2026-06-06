using ArquanixApi.Models;

namespace ArquanixApi.Services;


public interface IClaimStore
{
    IEnumerable<Claim> GetAll();

    Claim? GetById(int id);

    Claim Add(Claim claim);

    bool Update(Claim claim);

    bool Delete(int id);
}
