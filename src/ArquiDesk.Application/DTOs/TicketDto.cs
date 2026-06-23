using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Application.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string RequesterUserId { get; set; } = string.Empty;
    public TicketType Type { get; set; }
    public TicketPriority Priority { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public DateTime SlaDueAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResponsibleUserId { get; set; }
    public TicketStatus Status { get; set; }
    public bool IsOverdue { get; set; }
}
