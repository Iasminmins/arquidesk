using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class TicketChangeLog : AuditableEntity
{
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
