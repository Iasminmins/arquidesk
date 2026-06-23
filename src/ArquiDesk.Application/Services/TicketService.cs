using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Domain.Extensions;

namespace ArquiDesk.Application.Services;

public class TicketService(IUnitOfWork unitOfWork, INotificationService notificationService) : ITicketService
{
    public DateTime CalculateSlaDueAt(TicketType type, DateTime openedAt)
    {
        var hours = type switch
        {
            TicketType.Orcamento => 48,
            TicketType.AlteracaoDeProjeto => 72,
            TicketType.ProblemaNaInstalacao => 24,
            TicketType.ErroDeMedida => 24,
            TicketType.AssistenciaTecnica => 120,
            TicketType.Garantia => 120,
            TicketType.ProblemaNaProducao => 24,
            _ => 48
        };

        return openedAt.AddHours(hours);
    }

    public async Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var lastNumber = unitOfWork.Tickets.Query().OrderByDescending(x => x.Number).Select(x => x.Number).FirstOrDefault();
        ticket.Number = lastNumber + 1;
        ticket.OpenedAt = DateTime.UtcNow;
        ticket.SlaDueAt = CalculateSlaDueAt(ticket.Type, ticket.OpenedAt);

        await unitOfWork.Tickets.AddAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await notificationService.NotifyTicketCreatedAsync(ticket, cancellationToken);

        return ticket;
    }

    public async Task UpdateStatusAsync(Guid ticketId, TicketStatus status, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await unitOfWork.Tickets.GetByIdAsync(ticketId, cancellationToken)
            ?? throw new InvalidOperationException("Chamado não encontrado.");

        var oldStatus = ticket.Status;
        ticket.Status = status;
        ticket.UpdatedBy = userId;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.ResolvedAt = status == TicketStatus.Resolvido ? DateTime.UtcNow : ticket.ResolvedAt;

        await unitOfWork.TicketChangeLogs.AddAsync(new TicketChangeLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            FieldName = nameof(ticket.Status),
            OldValue = oldStatus.GetDisplayName(),
            NewValue = status.GetDisplayName(),
            CreatedBy = userId
        }, cancellationToken);

        unitOfWork.Tickets.Update(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await notificationService.NotifyStatusChangedAsync(ticket, oldStatus, cancellationToken);
    }
}
