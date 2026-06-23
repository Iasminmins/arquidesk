using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class Client : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
