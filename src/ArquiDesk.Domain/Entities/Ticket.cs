using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class Ticket : AuditableEntity
{
    public int Number { get; set; }
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public string RequesterUserId { get; set; } = string.Empty;
    public TicketType Type { get; set; }
    public TicketPriority Priority { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime SlaDueAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResponsibleUserId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Aberto;
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    public ICollection<TicketChangeLog> ChangeLogs { get; set; } = new List<TicketChangeLog>();

    public bool IsOverdue(DateTime utcNow) => Status is not TicketStatus.Resolvido and not TicketStatus.Cancelado && SlaDueAt < utcNow;
}
