using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Domain.Extensions;

namespace ArquiDesk.Infrastructure.Services;

public class NotificationService(IUnitOfWork unitOfWork) : INotificationService
{
    public async Task NotifyTicketCreatedAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticket.ResponsibleUserId))
        {
            return;
        }

        await unitOfWork.Notifications.AddAsync(new Notification
        {
            UserId = ticket.ResponsibleUserId,
            TicketId = ticket.Id,
            Title = $"Novo chamado #{ticket.Number}",
            Message = "Um novo chamado foi criado e aguarda acompanhamento."
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyStatusChangedAsync(Ticket ticket, TicketStatus oldStatus, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Notifications.AddAsync(new Notification
        {
            UserId = ticket.RequesterUserId,
            TicketId = ticket.Id,
            Title = $"Chamado #{ticket.Number} atualizado",
            Message = $"Status alterado de {oldStatus.GetDisplayName()} para {ticket.Status.GetDisplayName()}."
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
