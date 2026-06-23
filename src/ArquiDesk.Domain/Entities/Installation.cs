using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class Installation : AuditableEntity
{
    public string ClientName { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public DateTime? FactoryBillingDate { get; set; }
    public bool? OrderArrived { get; set; }
    public DateTime? InstallationDate { get; set; }
    public string? InstallerName { get; set; }
    public InstallationStatus Status { get; set; } = InstallationStatus.AguardandoFaturamento;
    public string? Notes { get; set; }
}
