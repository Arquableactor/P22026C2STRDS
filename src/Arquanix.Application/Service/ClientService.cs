using System.ComponentModel.DataAnnotations;
using Arquanix.Application.Contract;
using Arquanix.Application.Core;
using Arquanix.Application.Dtos.Clients;
using Arquanix.Domain.Entities;
using Arquanix.Infrastructure.Interfaces;

namespace Arquanix.Application.Service;


public class ClientService : IClientService
{
    private readonly IClientRepository _repository;
    private readonly IClaimRepository _claimRepository;

    public ClientService(IClientRepository repository, IClaimRepository claimRepository)
    {
        _repository = repository;
        _claimRepository = claimRepository;
    }

    public async Task<ServiceResult<List<ClientDto>>> GetAllAsync(bool? activos = null, string? busqueda = null)
    {
        var clients = await _repository.GetAllAsync();

        if (activos == true)
        {
            clients = clients.Where(c => c.IsActive).ToList();
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim().ToLowerInvariant();
            clients = clients
                .Where(c => c.Name.ToLowerInvariant().Contains(termino)
                    || c.Email.ToLowerInvariant().Contains(termino))
                .ToList();
        }

        var vigentesPorCliente = await VigentesPorClienteAsync();
        var dtos = clients
            .Select(c => ToDto(c, vigentesPorCliente.GetValueOrDefault(c.Id)))
            .ToList();

        return ServiceResult<List<ClientDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<ClientDto>> GetByIdAsync(int id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client is null)
        {
            return ServiceResult<ClientDto>.NotFound();
        }

        var vigentesPorCliente = await VigentesPorClienteAsync();
        return ServiceResult<ClientDto>.Ok(ToDto(client, vigentesPorCliente.GetValueOrDefault(client.Id)));
    }

    public async Task<ServiceResult<ClientDto>> CreateAsync(CreateClientDto dto)
    {
        var errors = Validate(dto.Name, dto.Email, dto.Phone);
        if (errors.Count > 0)
        {
            return ServiceResult<ClientDto>.Invalid(errors);
        }

        var client = new Client
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            IsActive = dto.IsActive,
        };

        var created = await _repository.CreateAsync(client);
        return ServiceResult<ClientDto>.Ok(ToDto(created, 0));
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateClientDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult.NotFound();
        }

        var errors = Validate(dto.Name, dto.Email, dto.Phone);
        if (errors.Count > 0)
        {
            return ServiceResult.Invalid(errors);
        }

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.IsActive = dto.IsActive;

        await _repository.UpdateAsync(existing);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? ServiceResult.Ok() : ServiceResult.NotFound();
    }

    private async Task<Dictionary<int, int>> VigentesPorClienteAsync()
    {
        var claims = await _claimRepository.GetAllAsync();
        return claims
            .Where(c => c.Status == ClaimStatus.Open || c.Status == ClaimStatus.InProgress)
            .GroupBy(c => c.ClientId)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static List<string> Validate(string name, string email, string? phone)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2 || name.Length > 100)
        {
            errors.Add("El nombre debe tener entre 2 y 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(email) || email.Length > 256 || !new EmailAddressAttribute().IsValid(email))
        {
            errors.Add("El correo no tiene un formato válido.");
        }

        if (!string.IsNullOrEmpty(phone) && phone.Length > 40)
        {
            errors.Add("El teléfono no puede exceder los 40 caracteres.");
        }

        return errors;
    }

    private static ClientDto ToDto(Client client, int reclamosVigentes) => new()
    {
        Id = client.Id,
        Name = client.Name,
        Email = client.Email,
        Phone = client.Phone,
        IsActive = client.IsActive,
        CreatedAt = client.CreatedAt,
        Rol = "Cliente",
        ReclamosVigentes = reclamosVigentes,
    };
}
