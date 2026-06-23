using ArquiDesk.Domain.Common;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
    public DbSet<TicketChangeLog> TicketChangeLogs => Set<TicketChangeLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Negotiation> Negotiations => Set<Negotiation>();
    public DbSet<Installation> Installations => Set<Installation>();
    public DbSet<AssistanceRequest> AssistanceRequests => Set<AssistanceRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Client>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Document).HasMaxLength(30);
            entity.Property(x => x.Email).HasMaxLength(160);
            entity.Property(x => x.Phone).HasMaxLength(40);
            entity.Property(x => x.Address).HasMaxLength(240);
        });

        builder.Entity<Project>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(240).IsRequired();
            entity.Property(x => x.ResponsibleUserId).HasMaxLength(450).IsRequired();
            entity.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.ClientId);
        });

        builder.Entity<Room>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(800);
            entity.HasOne(x => x.Project).WithMany(x => x.Rooms).HasForeignKey(x => x.ProjectId);
        });

        builder.Entity<Ticket>(entity =>
        {
            entity.HasIndex(x => x.Number).IsUnique();
            entity.Property(x => x.Description).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.RequesterUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.ResponsibleUserId).HasMaxLength(450);
            entity.HasOne(x => x.Project).WithMany(x => x.Tickets).HasForeignKey(x => x.ProjectId);
        });

        builder.Entity<TicketComment>(entity =>
        {
            entity.Property(x => x.Message).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.HasOne(x => x.Ticket).WithMany(x => x.Comments).HasForeignKey(x => x.TicketId);
        });

        builder.Entity<TicketAttachment>(entity =>
        {
            entity.Property(x => x.OriginalFileName).HasMaxLength(240).IsRequired();
            entity.Property(x => x.StoredFileName).HasMaxLength(240).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.UploadedByUserId).HasMaxLength(450).IsRequired();
            entity.HasOne(x => x.Ticket).WithMany(x => x.Attachments).HasForeignKey(x => x.TicketId);
        });

        builder.Entity<TicketChangeLog>(entity =>
        {
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.FieldName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.OldValue).HasMaxLength(500);
            entity.Property(x => x.NewValue).HasMaxLength(500);
            entity.HasOne(x => x.Ticket).WithMany(x => x.ChangeLogs).HasForeignKey(x => x.TicketId);
        });

        builder.Entity<Notification>(entity =>
        {
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.Ticket).WithMany().HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Lead>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Contact).HasMaxLength(60);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.Property(x => x.Observations).HasMaxLength(1000);
        });

        builder.Entity<Negotiation>(entity =>
        {
            entity.Property(x => x.ClientName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Contact).HasMaxLength(60);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.Property(x => x.Cost).HasColumnType("numeric(18,2)");
            entity.Property(x => x.LastOfferedValue).HasColumnType("numeric(18,2)");
            entity.Property(x => x.CashValue).HasColumnType("numeric(18,2)");
            entity.Property(x => x.UpdatedValue).HasColumnType("numeric(18,2)");
        });

        builder.Entity<Installation>(entity =>
        {
            entity.Property(x => x.ClientName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Contact).HasMaxLength(60);
            entity.Property(x => x.InstallerName).HasMaxLength(120);
            entity.Property(x => x.Notes).HasMaxLength(1000);
        });

        builder.Entity<AssistanceRequest>(entity =>
        {
            entity.Property(x => x.ClientName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Contact).HasMaxLength(60);
            entity.Property(x => x.Notes).HasMaxLength(1000);
        });

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAudit()
    {
        var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
        var compare = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(compare, parameter);
    }
}
