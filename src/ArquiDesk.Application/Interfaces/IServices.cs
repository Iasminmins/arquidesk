using ArquiDesk.Application.DTOs;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Application.Interfaces;

public interface ITicketService
{
    DateTime CalculateSlaDueAt(TicketType type, DateTime openedAt);
    Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid ticketId, TicketStatus status, string userId, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}

public interface INotificationService
{
    Task NotifyTicketCreatedAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task NotifyStatusChangedAsync(Ticket ticket, TicketStatus oldStatus, CancellationToken cancellationToken = default);
}

public interface IFileStorageService
{
    string[] AllowedExtensions { get; }
    Task<TicketAttachment> SaveTicketAttachmentAsync(Guid ticketId, string userId, Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
}
