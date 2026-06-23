using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class TicketAttachment : AuditableEntity
{
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
}
