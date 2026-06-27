using Arquanix.Domain.Entities;

namespace Arquanix.Infrastructure.Interfaces;


public interface IClaimRepository
{
    Task<List<Claim>> GetAllAsync();

    Task<Claim?> GetByIdAsync(int id);

    Task<Claim> CreateAsync(Claim claim);

    Task<bool> UpdateAsync(Claim claim);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}
