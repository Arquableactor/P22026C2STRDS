namespace Arquanix.Application.Dtos.Claims;


public class ClaimStatsDto
{
    public int Total { get; set; }

    public int Open { get; set; }

    public int InProgress { get; set; }

    public int Resolved { get; set; }

    public int Closed { get; set; }

    public int Critical { get; set; }

    public int PromedioDiasAtencion { get; set; }

    public int TotalClientes { get; set; }

    public int ClientesActivos { get; set; }
}
