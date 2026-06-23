using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Domain.Entities;

public class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Lead? Lead { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string ResponsibleUserId { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.EmOrcamento;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
