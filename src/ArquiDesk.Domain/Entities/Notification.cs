using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class Notification : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public Guid? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Read { get; set; }
    public DateTime? ReadAt { get; set; }
}
