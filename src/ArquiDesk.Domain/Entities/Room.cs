using ArquiDesk.Domain.Common;

namespace ArquiDesk.Domain.Entities;

public class Room : AuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
