using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Application.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime ExpectedDeliveryDate { get; set; } = DateTime.Today.AddDays(30);
    public string ResponsibleUserId { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
}
