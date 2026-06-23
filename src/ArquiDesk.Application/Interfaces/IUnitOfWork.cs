using ArquiDesk.Domain.Entities;

namespace ArquiDesk.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<Client> Clients { get; }
    IRepository<Project> Projects { get; }
    IRepository<Room> Rooms { get; }
    IRepository<Ticket> Tickets { get; }
    IRepository<TicketComment> TicketComments { get; }
    IRepository<TicketAttachment> TicketAttachments { get; }
    IRepository<TicketChangeLog> TicketChangeLogs { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<Lead> Leads { get; }
    IRepository<Negotiation> Negotiations { get; }
    IRepository<Installation> Installations { get; }
    IRepository<AssistanceRequest> AssistanceRequests { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
