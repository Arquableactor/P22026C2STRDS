using Arquanix.Application.Core;
using Arquanix.Application.Dtos.Clients;

namespace Arquanix.Application.Contract;


public interface IClientService
{
    Task<ServiceResult<List<ClientDto>>> GetAllAsync(bool? activos = null, string? busqueda = null);

    Task<ServiceResult<ClientDto>> GetByIdAsync(int id);

    Task<ServiceResult<ClientDto>> CreateAsync(CreateClientDto dto);

    Task<ServiceResult> UpdateAsync(int id, UpdateClientDto dto);

    Task<ServiceResult> DeleteAsync(int id);
}
