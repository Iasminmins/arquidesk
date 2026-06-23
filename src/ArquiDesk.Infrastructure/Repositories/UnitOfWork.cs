using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Infrastructure.Data;

namespace ArquiDesk.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public IRepository<Client> Clients { get; } = new Repository<Client>(context);
    public IRepository<Project> Projects { get; } = new Repository<Project>(context);
    public IRepository<Room> Rooms { get; } = new Repository<Room>(context);
    public IRepository<Ticket> Tickets { get; } = new Repository<Ticket>(context);
    public IRepository<TicketComment> TicketComments { get; } = new Repository<TicketComment>(context);
    public IRepository<TicketAttachment> TicketAttachments { get; } = new Repository<TicketAttachment>(context);
    public IRepository<TicketChangeLog> TicketChangeLogs { get; } = new Repository<TicketChangeLog>(context);
    public IRepository<Notification> Notifications { get; } = new Repository<Notification>(context);
    public IRepository<Lead> Leads { get; } = new Repository<Lead>(context);
    public IRepository<Negotiation> Negotiations { get; } = new Repository<Negotiation>(context);
    public IRepository<Installation> Installations { get; } = new Repository<Installation>(context);
    public IRepository<AssistanceRequest> AssistanceRequests { get; } = new Repository<AssistanceRequest>(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
