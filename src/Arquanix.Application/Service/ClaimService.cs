using Arquanix.Application.Contract;
using Arquanix.Application.Core;
using Arquanix.Application.Dtos.Claims;
using Arquanix.Domain.Entities;
using Arquanix.Infrastructure.Interfaces;

namespace Arquanix.Application.Service;


public class ClaimService : IClaimService
{
    private readonly IClaimRepository _repository;
    private readonly IClientRepository _clientRepository;

    public ClaimService(IClaimRepository repository, IClientRepository clientRepository)
    {
        _repository = repository;
        _clientRepository = clientRepository;
    }

    public async Task<ServiceResult<List<ClaimDto>>> GetAllAsync(int? clientId = null, ClaimStatus? status = null)
    {
        var claims = await _repository.GetAllAsync();

        if (clientId.HasValue)
        {
            claims = claims.Where(c => c.ClientId == clientId.Value).ToList();
        }

        if (status.HasValue)
        {
            claims = claims.Where(c => c.Status == status.Value).ToList();
        }

        var nombres = await NombresPorClienteAsync();
        var dtos = claims.Select(c => ToDto(c, nombres.GetValueOrDefault(c.ClientId))).ToList();

        return ServiceResult<List<ClaimDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<ClaimDto>> GetByIdAsync(int id)
    {
        var claim = await _repository.GetByIdAsync(id);
        if (claim is null)
        {
            return ServiceResult<ClaimDto>.NotFound();
        }

        var client = await _clientRepository.GetByIdAsync(claim.ClientId);
        return ServiceResult<ClaimDto>.Ok(ToDto(claim, client?.Name));
    }

    public async Task<ServiceResult<ClaimDto>> CreateAsync(CreateClaimDto dto)
    {
        var errors = Validate(dto.ClientId, dto.Title, dto.Description, dto.Status, dto.Priority);
        if (errors.Count > 0)
        {
            return ServiceResult<ClaimDto>.Invalid(errors);
        }

        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client is null)
        {
            return ServiceResult<ClaimDto>.Invalid("El cliente indicado no existe.");
        }

        var claim = new Claim
        {
            ClientId = dto.ClientId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
        };

        if (dto.Status == ClaimStatus.Closed)
        {
            claim.Close();
        }

        var created = await _repository.CreateAsync(claim);
        return ServiceResult<ClaimDto>.Ok(ToDto(created, client.Name));
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateClaimDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult.NotFound();
        }

        var errors = Validate(dto.ClientId, dto.Title, dto.Description, dto.Status, dto.Priority);
        if (errors.Count > 0)
        {
            return ServiceResult.Invalid(errors);
        }

        if (!await _clientRepository.ExistsAsync(dto.ClientId))
        {
            return ServiceResult.Invalid("El cliente indicado no existe.");
        }

        existing.ClientId = dto.ClientId;
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Priority = dto.Priority;

        if (dto.Status == ClaimStatus.Closed)
        {
            existing.Close(existing.ClosedAt ?? DateTime.UtcNow);
        }
        else
        {
            existing.Status = dto.Status;
            existing.ClosedAt = null;
        }

        await _repository.UpdateAsync(existing);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? ServiceResult.Ok() : ServiceResult.NotFound();
    }

    public async Task<ServiceResult<ClaimStatsDto>> GetStatsAsync()
    {
        var claims = await _repository.GetAllAsync();
        var clients = await _clientRepository.GetAllAsync();

        var stats = new ClaimStatsDto
        {
            Total = claims.Count,
            Open = claims.Count(c => c.Status == ClaimStatus.Open),
            InProgress = claims.Count(c => c.Status == ClaimStatus.InProgress),
            Resolved = claims.Count(c => c.Status == ClaimStatus.Resolved),
            Closed = claims.Count(c => c.Status == ClaimStatus.Closed),
            Critical = claims.Count(c => c.Priority == ClaimPriority.Critical),
            PromedioDiasAtencion = claims.Count > 0
                ? (int)Math.Round(claims.Average(c => (double)DiasDeAtencion(c)))
                : 0,
            TotalClientes = clients.Count,
            ClientesActivos = clients.Count(c => c.IsActive),
        };

        return ServiceResult<ClaimStatsDto>.Ok(stats);
    }

    private async Task<Dictionary<int, string>> NombresPorClienteAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        return clients.ToDictionary(c => c.Id, c => c.Name);
    }

    private static int DiasDeAtencion(Claim claim)
    {
        var fin = claim.ClosedAt ?? DateTime.UtcNow;
        var dias = (int)Math.Floor((fin - claim.CreatedAt).TotalDays);
        return dias < 0 ? 0 : dias;
    }

    private static List<string> Validate(int clientId, string title, string description, ClaimStatus status, ClaimPriority priority)
    {
        var errors = new List<string>();

        if (clientId <= 0)
        {
            errors.Add("El ClientId debe ser un identificador válido.");
        }

        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 3 || title.Length > 120)
        {
            errors.Add("El título debe tener entre 3 y 120 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length < 3 || description.Length > 1000)
        {
            errors.Add("La descripción debe tener entre 3 y 1000 caracteres.");
        }

        if (!Enum.IsDefined(typeof(ClaimStatus), status))
        {
            errors.Add("El estado del reclamo no es válido.");
        }

        if (!Enum.IsDefined(typeof(ClaimPriority), priority))
        {
            errors.Add("La prioridad del reclamo no es válida.");
        }

        return errors;
    }

    private static ClaimDto ToDto(Claim claim, string? clientName) => new()
    {
        Id = claim.Id,
        ClientId = claim.ClientId,
        ClientName = clientName,
        Title = claim.Title,
        Description = claim.Description,
        Status = claim.Status,
        Priority = claim.Priority,
        CreatedAt = claim.CreatedAt,
        ClosedAt = claim.ClosedAt,
        DiasDeAtencion = DiasDeAtencion(claim),
        Resumen = claim.GetSummary(),
    };
}
