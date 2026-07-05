using Arquanix.Application.Core;
using Arquanix.Application.Dtos.Claims;

namespace Arquanix.Application.Contract;


public interface IClaimService
{
    Task<ServiceResult<List<ClaimDto>>> GetAllAsync();

    Task<ServiceResult<ClaimDto>> GetByIdAsync(int id);

    Task<ServiceResult<ClaimDto>> CreateAsync(CreateClaimDto dto);

    Task<ServiceResult> UpdateAsync(int id, UpdateClaimDto dto);

    Task<ServiceResult> DeleteAsync(int id);
}
