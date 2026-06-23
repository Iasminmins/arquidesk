using System.Linq.Expressions;
using ArquiDesk.Domain.Common;

namespace ArquiDesk.Application.Interfaces;

public interface IRepository<T> where T : AuditableEntity
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void SoftDelete(T entity, string? userId = null);
}
