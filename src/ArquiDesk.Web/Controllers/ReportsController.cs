using System.Text;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Domain.Extensions;
using ArquiDesk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto},{UserRoles.Projetista}")]
public class ReportsController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var closedStatuses = new[] { TicketStatus.Resolvido, TicketStatus.Cancelado };
        var now = DateTime.UtcNow;

        var tickets = unitOfWork.Tickets.Query().Include(x => x.Project).AsNoTracking();
        var projects = unitOfWork.Projects.Query().AsNoTracking();
        var ticketsByStatus = await tickets
            .GroupBy(x => x.Status)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        var ticketsByPriority = await tickets
            .GroupBy(x => x.Priority)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        var recentTickets = await tickets
            .OrderByDescending(x => x.OpenedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var model = new ReportsViewModel
        {
            TotalTickets = await tickets.CountAsync(cancellationToken),
            OpenTickets = await tickets.CountAsync(x => !closedStatuses.Contains(x.Status), cancellationToken),
            OverdueTickets = await tickets.CountAsync(x => !closedStatuses.Contains(x.Status) && x.SlaDueAt < now, cancellationToken),
            TotalProjects = await projects.CountAsync(cancellationToken),
            ActiveProjects = await projects.CountAsync(x => x.Status != ProjectStatus.Finalizado, cancellationToken),
            TotalLeads = await unitOfWork.Leads.Query().CountAsync(cancellationToken),
            TicketsByStatus = ticketsByStatus.ToDictionary(x => x.Key.GetDisplayName(), x => x.Value),
            TicketsByPriority = ticketsByPriority.ToDictionary(x => x.Key.GetDisplayName(), x => x.Value),
            RecentTickets = recentTickets
                .Select(x => new RecentTicketReportItem
                {
                    Number = x.Number,
                    Project = x.Project != null ? x.Project.Name : string.Empty,
                    Type = x.Type.GetDisplayName(),
                    Priority = x.Priority.GetDisplayName(),
                    Status = x.Status.GetDisplayName(),
                    OpenedAt = x.OpenedAt,
                    SlaDueAt = x.SlaDueAt
                })
                .ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> TicketsCsv(CancellationToken cancellationToken)
    {
        var tickets = await unitOfWork.Tickets.Query()
            .Include(x => x.Project)
            .OrderByDescending(x => x.OpenedAt)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Número;Projeto;Tipo;Prioridade;Status;Abertura;SLA");
        foreach (var ticket in tickets)
        {
            csv.AppendLine($"{ticket.Number};{Escape(ticket.Project?.Name)};{ticket.Type.GetDisplayName()};{ticket.Priority.GetDisplayName()};{ticket.Status.GetDisplayName()};{ticket.OpenedAt:dd/MM/yyyy HH:mm};{ticket.SlaDueAt:dd/MM/yyyy HH:mm}");
        }

        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv", "chamados-arquidesk.csv");
    }

    private static string Escape(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace(";", ",").ReplaceLineEndings(" ");
}
