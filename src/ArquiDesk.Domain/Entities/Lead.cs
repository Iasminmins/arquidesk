using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class Lead : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public bool? Answered { get; set; }
    public bool? Interested { get; set; }
    public bool? ProjectSent { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.Novo;
    public DateTime? NextFollowUpAt { get; set; }
    public string? OwnerName { get; set; }
    public string? Observations { get; set; }
}
