using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class TicketComment : AuditableEntity
{
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}
