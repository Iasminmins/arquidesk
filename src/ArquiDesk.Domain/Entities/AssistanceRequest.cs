using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class AssistanceRequest : AuditableEntity
{
    public string ClientName { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public bool? VisitCompleted { get; set; }
    public bool? Completed { get; set; }
    public bool? OrderPlaced { get; set; }
    public DateTime? AssistanceDate { get; set; }
    public AssistanceStatus Status { get; set; } = AssistanceStatus.Aberta;
    public string? Notes { get; set; }
}
