using System.ComponentModel.DataAnnotations;
using Arquanix.Domain.Entities;

namespace ArquanixApi.Dtos;


public class CreateClaimDto
{
    [Required(ErrorMessage = "El ClientId es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ClientId debe ser un identificador válido.")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 120 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(ClaimStatus), ErrorMessage = "El estado del reclamo no es válido.")]
    public ClaimStatus Status { get; set; } = ClaimStatus.Open;

    [EnumDataType(typeof(ClaimPriority), ErrorMessage = "La prioridad del reclamo no es válida.")]
    public ClaimPriority Priority { get; set; } = ClaimPriority.Medium;
}
