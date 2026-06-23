namespace ArquiDesk.Application.DTOs;

public record DashboardDto(
    int OpenTickets,
    int OverdueTickets,
    int ActiveProjects,
    int DeliveredProjects,
    double AverageResolutionHours,
    int SlaMet,
    int SlaOverdue,
    IReadOnlyDictionary<string, int> TicketsByStatus,
    IReadOnlyDictionary<string, int> TicketsByPriority,
    IReadOnlyDictionary<string, int> TicketsByType);
