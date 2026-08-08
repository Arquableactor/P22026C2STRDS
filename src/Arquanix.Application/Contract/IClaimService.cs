using Arquanix.Application.Core;
using Arquanix.Application.Dtos.Claims;
using Arquanix.Domain.Entities;

namespace Arquanix.Application.Contract;


public interface IClaimService
{
    Task<ServiceResult<List<ClaimDto>>> GetAllAsync(int? clientId = null, ClaimStatus? status = null);

    Task<ServiceResult<ClaimDto>> GetByIdAsync(int id);

    Task<ServiceResult<ClaimDto>> CreateAsync(CreateClaimDto dto);

    Task<ServiceResult> UpdateAsync(int id, UpdateClaimDto dto);

    Task<ServiceResult> DeleteAsync(int id);

    Task<ServiceResult<ClaimStatsDto>> GetStatsAsync();
}
