using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class Negotiation : AuditableEntity
{
    public string ClientName { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public decimal? Cost { get; set; }
    public decimal? LastOfferedValue { get; set; }
    public decimal? CashValue { get; set; }
    public decimal? UpdatedValue { get; set; }
    public string? OwnerName { get; set; }
    public NegotiationStatus Status { get; set; } = NegotiationStatus.Aberta;
    public DateTime? NextContactAt { get; set; }
    public string? Notes { get; set; }
}
