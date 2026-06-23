using ArquiDesk.Application.DTOs;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Application.Services;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var tickets = unitOfWork.Tickets.Query().AsNoTracking();
        var projects = unitOfWork.Projects.Query().AsNoTracking();

        var closedStatuses = new[] { TicketStatus.Resolvido, TicketStatus.Cancelado };
        var resolvedTickets = await tickets
            .Where(x => x.ResolvedAt != null)
            .Select(x => new { x.OpenedAt, x.ResolvedAt })
            .ToListAsync(cancellationToken);

        var averageHours = resolvedTickets.Count > 0
            ? resolvedTickets.Average(x => (x.ResolvedAt!.Value - x.OpenedAt).TotalHours)
            : 0;

        var byStatus = await tickets
            .GroupBy(x => x.Status)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        var byPriority = await tickets
            .GroupBy(x => x.Priority)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        var byType = await tickets
            .GroupBy(x => x.Type)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);

        return new DashboardDto(
            OpenTickets: await tickets.CountAsync(x => !closedStatuses.Contains(x.Status), cancellationToken),
            OverdueTickets: await tickets.CountAsync(x => !closedStatuses.Contains(x.Status) && x.SlaDueAt < now, cancellationToken),
            ActiveProjects: await projects.CountAsync(x => x.Status != ProjectStatus.Finalizado, cancellationToken),
            DeliveredProjects: await projects.CountAsync(x => x.Status == ProjectStatus.Entregue || x.Status == ProjectStatus.Finalizado, cancellationToken),
            AverageResolutionHours: averageHours,
            SlaMet: await tickets.CountAsync(x => x.ResolvedAt != null && x.ResolvedAt <= x.SlaDueAt, cancellationToken),
            SlaOverdue: await tickets.CountAsync(x => x.SlaDueAt < now && !closedStatuses.Contains(x.Status), cancellationToken),
            TicketsByStatus: byStatus.ToDictionary(x => x.Key.GetDisplayName(), x => x.Value),
            TicketsByPriority: byPriority.ToDictionary(x => x.Key.GetDisplayName(), x => x.Value),
            TicketsByType: byType.ToDictionary(x => x.Key.GetDisplayName(), x => x.Value));
    }
}
